using TicTacToe.Core;

namespace TicTacToe.Tests;

public class BoardTests
{
    private readonly Board _board;

    public BoardTests()
    {
        _board = new Board(3);
    }

    // ── Lesson tests ──────────────────────────────────────────────────────────

    [Fact]
    public void NewBoard_IsAllEmpty()
    {
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            Assert.Equal('.', _board.GetCell(i, j));
    }

    [Fact]
    public void IsMoveValid_OnEmptyCell_ReturnsTrue()
    {
        Assert.True(_board.IsMoveValid(1));
    }

    [Fact]
    public void IsMoveValid_AfterPlacingMove_ReturnsFalse()
    {
        _board.PlaceMove(1, 'X');

        Assert.False(_board.IsMoveValid(1));
    }

    [Fact]
    public void PlaceMove_SetsCorrectCell()
    {
        _board.PlaceMove(5, 'X');

        // Position 5 → row 1, col 1 on a 3×3 board
        Assert.Equal('X', _board.GetCell(1, 1));
    }

    [Theory]
    [InlineData(1, 2, 3)] // row 0
    [InlineData(4, 5, 6)] // row 1
    [InlineData(7, 8, 9)] // row 2
    [InlineData(1, 4, 7)] // column 0
    [InlineData(2, 5, 8)] // column 1
    [InlineData(3, 6, 9)] // column 2
    [InlineData(1, 5, 9)] // diagonal ↘
    [InlineData(3, 5, 7)] // diagonal ↙
    public void CheckWin_DetectsAllWinningCombinations(int pos1, int pos2, int pos3)
    {
        _board.PlaceMove(pos1, 'X');
        _board.PlaceMove(pos2, 'X');
        _board.PlaceMove(pos3, 'X');

        Assert.True(_board.CheckWin('X'));
    }

    [Fact]
    public void CheckWin_WithNoMoves_ReturnsFalse()
    {
        Assert.False(_board.CheckWin('X'));
    }

    // ── Homework tests ────────────────────────────────────────────────────────

    [Fact]
    public void Board_Size_ReturnsConstructorValue()
    {
        Assert.Equal(3, _board.Size);
    }

    [Fact]
    public void IsDraw_OnEmptyBoard_ReturnsFalse()
    {
        Assert.False(_board.IsDraw());
    }

    [Fact]
    public void IsDraw_WhenAllCellsFilled_ReturnsTrue()
    {
        // Fill every cell — no need for a legal game sequence
        for (var pos = 1; pos <= 9; pos++)
            _board.PlaceMove(pos, 'X');

        Assert.True(_board.IsDraw());
    }

    [Fact]
    public void ToFlatString_OnNewBoard_ReturnsAllDots()
    {
        Assert.Equal(".........", _board.ToFlatString());
    }

    [Fact]
    public void ToFlatString_AfterPlacingMove_ReflectsSymbol()
    {
        // Position 1 is the first character of the flat string (row 0, col 0)
        _board.PlaceMove(1, 'X');

        Assert.Equal('X', _board.ToFlatString()[0]);
    }

    [Fact]
    public void ClearCell_AfterPlacingMove_RestoresDotAtThatCell()
    {
        _board.PlaceMove(5, 'X'); // row 1, col 1
        _board.ClearCell(1, 1);

        Assert.Equal('.', _board.GetCell(1, 1));
    }
}