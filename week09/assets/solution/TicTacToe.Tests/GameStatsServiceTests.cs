using TicTacToe.Core;

namespace TicTacToe.Tests;

public class GameStatsServiceTests
{
    private readonly GameStatsService _service;

    public GameStatsServiceTests()
    {
        _service = new GameStatsService();
    }

    [Fact]
    public void AddMove_AppearsInBothHistoryLists()
    {
        var move = new Move { Position = 1, Symbol = 'X', Timestamp = DateTime.Now };

        _service.AddMove(move);

        Assert.Single(_service.MoveHistory);
        Assert.Single(_service.GlobalMoveHistory);
    }

    [Fact]
    public void ClearHistory_RemovesFromMoveHistoryButNotGlobalHistory()
    {
        _service.AddMove(new Move { Position = 1, Symbol = 'X', Timestamp = DateTime.Now });
        _service.AddMove(new Move { Position = 2, Symbol = 'O', Timestamp = DateTime.Now });

        _service.ClearHistory();

        Assert.Empty(_service.MoveHistory);
        Assert.Equal(2, _service.GlobalMoveHistory.Count);
    }
}