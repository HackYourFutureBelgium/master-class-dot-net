using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TicTacToe.Core;

namespace TicTacToe.Tests;

public class GameEngineMoqTests
{
    private GameEngine CreateEngine(IGameStatsService statsService)
    {
        var engine = new GameEngine(NullLogger<GameEngine>.Instance, statsService);
        engine.SetPlayers(new Player("Alice", 'X'), new Player("Bob", 'O'));
        engine.SetBoardSize(3);
        return engine;
    }

    [Fact]
    public void TryPlayMove_ValidMove_CallsAddMoveOnce()
    {
        var mockStats = new Mock<IGameStatsService>();
        var engine = CreateEngine(mockStats.Object);

        engine.TryPlayMove(1);

        mockStats.Verify(s => s.AddMove(It.IsAny<Move>()), Times.Once);
    }

    [Fact]
    public void TryPlayMove_InvalidMove_DoesNotCallAddMove()
    {
        var mockStats = new Mock<IGameStatsService>();
        var engine = CreateEngine(mockStats.Object);

        engine.TryPlayMove(1); // Alice plays at position 1
        engine.TryPlayMove(1); // Bob tries the same cell — invalid

        // AddMove should have been called exactly once (for Alice's valid move only)
        mockStats.Verify(s => s.AddMove(It.IsAny<Move>()), Times.Once);
    }

    [Fact]
    public void TryPlayMove_ValidMove_PassesCorrectMoveToAddMove()
    {
        var mockStats = new Mock<IGameStatsService>();
        var engine = CreateEngine(mockStats.Object);

        engine.TryPlayMove(5); // Alice plays at position 5 with symbol 'X'

        mockStats.Verify(
            s => s.AddMove(It.Is<Move>(m => m.Position == 5 && m.Symbol == 'X')),
            Times.Once);
    }
}
