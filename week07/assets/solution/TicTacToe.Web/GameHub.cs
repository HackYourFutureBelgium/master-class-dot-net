using Microsoft.AspNetCore.SignalR;
using TicTacToe.Core;
using TicTacToe.Core.DataTransferObjects;

namespace TicTacToe.Web;

public interface IGameClient
{
    Task StateUpdated(GameStateDto state);
    Task LobbyUpdated(LobbyDto lobby);
    Task Error(string message);
}

public sealed class LobbyDto
{
    public string GameId { get; init; } = "";
    public string? PlayerXConnectionId { get; init; }
    public string? PlayerOConnectionId { get; init; }
    public int Spectators { get; init; }
}

public sealed class GameHub : Hub<IGameClient>
{
    private readonly GameRoomManager _rooms;
    private readonly ILogger<GameHub> _logger;
    private readonly IServiceProvider _sp;

    public GameHub(GameRoomManager rooms, ILogger<GameHub> logger, IServiceProvider sp)
    {
        _rooms = rooms;
        _logger = logger;
        _sp = sp;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var kv in GetRoomsSnapshot())
        {
            if (!kv.Value.Connections.ContainsKey(Context.ConnectionId))
                continue;

            kv.Value.Connections.TryRemove(Context.ConnectionId, out _);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, kv.Key);
            await BroadcastLobby(kv.Value);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private IReadOnlyDictionary<string, GameRoomManager.GameRoom> GetRoomsSnapshot()
    {
        // Not ideal, but fine for small course projects.
        // Alternative: track room id per connection via Context.Items.
        var field = typeof(GameRoomManager).GetField("_rooms", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return ((System.Collections.Concurrent.ConcurrentDictionary<string, GameRoomManager.GameRoom>)field!.GetValue(_rooms)!).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task CreateOrJoin(string gameId, string playerName, int boardSize = 3)
    {
        var room = _rooms.GetOrCreate(gameId, () =>
        {
            // Create an engine for this room (scoped deps)
            using var scope = _sp.CreateScope();
            var engine = scope.ServiceProvider.GetRequiredService<GameEngine>();

            // Start with placeholder players (names will be set when players join)
            engine.SetPlayers(new Player("Player X", 'X'), new Player("Player O", 'O'));
            engine.SetBoardSize(boardSize);

            return new GameRoomManager.GameRoom
            {
                GameId = gameId,
                Engine = engine
            };
        });

        await room.Gate.WaitAsync();
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            // Assign X then O, otherwise spectator
            var assigned = AssignSymbol(room, Context.ConnectionId);
            room.Connections[Context.ConnectionId] = assigned;

            // Apply name if X or O
            if (assigned == 'X')
                room.Engine.Player1 = new Player(playerName, 'X');
            else if (assigned == 'O')
                room.Engine.Player2 = new Player(playerName, 'O');

            // Ensure current player is set (if first join)
            room.Engine.SetPlayers(room.Engine.Player1, room.Engine.Player2);
        }
        finally
        {
            room.Gate.Release();
        }

        await BroadcastLobby(room);
        await Clients.Caller.StateUpdated(room.Engine.ToDto(gameId));
        await Clients.Group(gameId).StateUpdated(room.Engine.ToDto(gameId));
    }

    public async Task PlayMove(string gameId, int position)
    {
        if (!_rooms.TryGet(gameId, out var room))
        {
            await Clients.Caller.Error("Game not found.");
            return;
        }

        await room.Gate.WaitAsync();
        try
        {
            var symbol = room.Connections.TryGetValue(Context.ConnectionId, out var s) ? s : null;
            if (symbol is null)
            {
                await Clients.Caller.Error("Spectators cannot play.");
                return;
            }

            if (!room.Engine.IsPlayersTurn(symbol.Value))
            {
                await Clients.Caller.Error("Not your turn.");
                return;
            }

            var ok = room.Engine.TryPlayMove(position);
            if (!ok)
            {
                await Clients.Caller.Error("Invalid move.");
                return;
            }
        }
        finally
        {
            room.Gate.Release();
        }

        await Clients.Group(gameId).StateUpdated(room.Engine.ToDto(gameId));
    }

    public async Task Undo(string gameId)
    {
        if (!_rooms.TryGet(gameId, out var room))
        {
            await Clients.Caller.Error("Game not found.");
            return;
        }

        await room.Gate.WaitAsync();
        try
        {
            // Optional: only allow undo for the player who just played OR for both.
            room.Engine.TryUndoLastMove();
        }
        finally
        {
            room.Gate.Release();
        }

        await Clients.Group(gameId).StateUpdated(room.Engine.ToDto(gameId));
    }

    public async Task NewRound(string gameId, int boardSize)
    {
        if (!_rooms.TryGet(gameId, out var room))
        {
            await Clients.Caller.Error("Game not found.");
            return;
        }

        await room.Gate.WaitAsync();
        try
        {
            room.Engine.SetBoardSize(boardSize);
            room.Engine.History.ClearHistory();
        }
        finally
        {
            room.Gate.Release();
        }

        await Clients.Group(gameId).StateUpdated(room.Engine.ToDto(gameId));
    }

    private static char? AssignSymbol(GameRoomManager.GameRoom room, string connectionId)
    {
        // If already assigned, keep
        if (room.Connections.TryGetValue(connectionId, out var existing))
            return existing;

        var hasX = room.Connections.Values.Any(v => v == 'X');
        var hasO = room.Connections.Values.Any(v => v == 'O');

        if (!hasX) return 'X';
        if (!hasO) return 'O';
        return null; // spectator
    }

    private Task BroadcastLobby(GameRoomManager.GameRoom room)
    {
        string? x = room.Connections.FirstOrDefault(kv => kv.Value == 'X').Key;
        string? o = room.Connections.FirstOrDefault(kv => kv.Value == 'O').Key;
        var spectators = room.Connections.Count(kv => kv.Value is null);

        var dto = new LobbyDto
        {
            GameId = room.GameId,
            PlayerXConnectionId = string.IsNullOrWhiteSpace(x) ? null : x,
            PlayerOConnectionId = string.IsNullOrWhiteSpace(o) ? null : o,
            Spectators = spectators
        };

        return Clients.Group(room.GameId).LobbyUpdated(dto);
    }
}
