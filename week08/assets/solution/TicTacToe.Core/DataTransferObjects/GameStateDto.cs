namespace TicTacToe.Core.DataTransferObjects;

public sealed class GameStateDto
{
    public string GameId { get; init; } = "";
    public int BoardSize { get; init; }
    public string BoardFlat { get; init; } = ""; // row-major, '.' for empty
    public GameStatus Status { get; init; }

    public string PlayerXName { get; init; } = "";
    public string PlayerOName { get; init; } = "";

    public char CurrentSymbol { get; init; } // 'X' or 'O'
    public string? WinnerName { get; init; }

    public IReadOnlyList<MoveDto> Moves { get; init; } = Array.Empty<MoveDto>();
}
