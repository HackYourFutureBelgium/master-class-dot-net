using System.Collections.Concurrent;
using TicTacToe.Core;

namespace TicTacToe.Web;

public sealed class GameRoomManager
{
    private readonly ConcurrentDictionary<string, GameRoom> _rooms = new();

    public GameRoom GetOrCreate(string gameId, Func<GameRoom> factory)
        => _rooms.GetOrAdd(gameId, _ => factory());

    public bool TryGet(string gameId, out GameRoom room) => _rooms.TryGetValue(gameId, out room!);

    public sealed class GameRoom
    {
        public required string GameId { get; init; }
        public required GameEngine Engine { get; init; }

        // ConnectionId -> assigned symbol ('X'/'O') or null for spectator
        public ConcurrentDictionary<string, char?> Connections { get; } = new();

        // Prevent concurrent move races
        public SemaphoreSlim Gate { get; } = new(1, 1);
    }
}
