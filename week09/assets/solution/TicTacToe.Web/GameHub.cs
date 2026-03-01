using Microsoft.AspNetCore.SignalR;

namespace TicTacToe.Web;

public class GameHub : Hub
{
    private readonly GameRoomManager _roomManager;

    public GameHub(GameRoomManager roomManager)
    {
        _roomManager = roomManager;
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task JoinLobby()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
        // Send the current snapshot immediately so the page loads with data
        await Clients.Caller.SendAsync("LobbyUpdated", _roomManager.GetActiveSummaries());
    }
}
