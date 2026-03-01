using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Core;

namespace TicTacToe.Tests;

public class GameEngineTests
{
    private GameEngine CreateEngine()
    {
        var engine = new GameEngine(NullLogger<GameEngine>.Instance, new GameStatsService());
        engine.SetPlayers(new Player("Alice", 'X'), new Player("Bob", 'O'));
        engine.SetBoardSize(3);
        return engine;
    }

    // ── Lesson tests ──────────────────────────────────────────────────────────

    [Fact]
    public void SetPlayers_FirstPlayerIsCurrentPlayer()
    {
        var engine = CreateEngine();

        Assert.Equal("Alice", engine.CurrentPlayer.Name);
    }

    [Fact]
    public void TryPlayMove_ValidMove_ReturnsTrue()
    {
        var engine = CreateEngine();

        Assert.True(engine.TryPlayMove(1));
    }

    [Fact]
    public void TryPlayMove_AfterValidMove_SwitchesPlayer()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice

        Assert.Equal("Bob", engine.CurrentPlayer.Name);
    }

    [Fact]
    public void TryPlayMove_OnOccupiedCell_ReturnsFalse()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice
        var result = engine.TryPlayMove(1); // Bob tries same cell

        Assert.False(result);
    }

    [Fact]
    public void TryPlayMove_WinningMove_SetsStatusToWin()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice
        engine.TryPlayMove(4); // Bob
        engine.TryPlayMove(2); // Alice
        engine.TryPlayMove(5); // Bob
        engine.TryPlayMove(3); // Alice — top row complete

        Assert.Equal(GameStatus.Win, engine.Status);
    }

    [Fact]
    public void TryPlayMove_WinningMove_IncrementsWinnerWins()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice
        engine.TryPlayMove(4); // Bob
        engine.TryPlayMove(2); // Alice
        engine.TryPlayMove(5); // Bob
        engine.TryPlayMove(3); // Alice wins

        Assert.Equal(1, engine.Player1.Wins);
    }

    [Fact]
    public void TryPlayMove_DrawSequence_SetsStatusToDraw()
    {
        var engine = CreateEngine();

        // X | O | X
        // O | X | X
        // O | X | O  — board full, no winner
        engine.TryPlayMove(1); // X
        engine.TryPlayMove(2); // O
        engine.TryPlayMove(3); // X
        engine.TryPlayMove(4); // O
        engine.TryPlayMove(5); // X
        engine.TryPlayMove(9); // O
        engine.TryPlayMove(6); // X
        engine.TryPlayMove(7); // O
        engine.TryPlayMove(8); // X

        Assert.Equal(GameStatus.Draw, engine.Status);
    }

    // ── Homework tests ────────────────────────────────────────────────────────

    [Fact]
    public void TryPlayMove_WithoutSetup_ReturnsFalse()
    {
        // No SetPlayers, no SetBoardSize — Board is null
        var engine = new GameEngine(
            NullLogger<GameEngine>.Instance,
            new GameStatsService());

        Assert.False(engine.TryPlayMove(1));
    }

    [Fact]
    public void TryUndoLastMove_WithNoMoveHistory_ReturnsFalse()
    {
        var engine = CreateEngine();

        Assert.False(engine.TryUndoLastMove());
    }

    [Fact]
    public void TryUndoLastMove_AfterOneMove_RestoresCellToEmpty()
    {
        var engine = CreateEngine();
        engine.TryPlayMove(1); // Alice places X at position 1 (row 0, col 0)

        var result = engine.TryUndoLastMove();

        Assert.True(result);
        Assert.Equal('.', engine.Board.GetCell(0, 0));
        Assert.Empty(engine.History.MoveHistory);
        Assert.Equal("Alice", engine.CurrentPlayer.Name);
    }

    [Fact]
    public void TryUndoLastMove_AfterWinningMove_ResetsStatusToInProgress()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice
        engine.TryPlayMove(4); // Bob
        engine.TryPlayMove(2); // Alice
        engine.TryPlayMove(5); // Bob
        engine.TryPlayMove(3); // Alice wins

        Assert.Equal(GameStatus.Win, engine.Status);

        engine.TryUndoLastMove();

        Assert.Equal(GameStatus.InProgress, engine.Status);
    }
}