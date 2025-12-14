namespace TicTacToe.Core.DataTransferObjects;

public sealed class MoveDto
{
    public int Position { get; init; }
    public char Symbol { get; init; }
    public DateTime Timestamp { get; init; }
}
