using System.Collections.Concurrent;
using TicTacToe.Core;

namespace TicTacToe.Web;

public record GameSummary(
    string GameId,
    string Player1Name,
    int Player1Wins,
    string Player2Name,
    int Player2Wins,
    string Status,
    string CurrentPlayer
);

public class GameRoomManager
{
    private readonly ConcurrentDictionary<string, GameEngine> _rooms = new();
    private readonly ILoggerFactory _loggerFactory;

    public GameRoomManager(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public GameEngine GetOrCreate(string gameId)
        => _rooms.GetOrAdd(
            gameId,
            _ => new GameEngine(_loggerFactory.CreateLogger<GameEngine>(), new GameStatsService())
        );

    public IEnumerable<GameSummary> GetActiveSummaries()
        => _rooms
            .Where(kv => kv.Value.Board != null)
            .Select(kv =>
            {
                var e = kv.Value;
                return new GameSummary(
                    kv.Key,
                    e.Player1?.Name ?? "?",
                    e.Player1?.Wins ?? 0,
                    e.Player2?.Name ?? "?",
                    e.Player2?.Wins ?? 0,
                    e.Status.ToString(),
                    e.CurrentPlayer?.Name ?? "?"
                );
            });
}
