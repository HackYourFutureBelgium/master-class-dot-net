namespace TicTacToe.Core;

public class GameStatsService
{
    public List<Move> MoveHistory { get; } = new();
    public List<Move> GlobalMoveHistory { get; } = new();

    public void AddMove(Move move)
    {
        MoveHistory.Add(move);
        GlobalMoveHistory.Add(move);
    }

    public void ClearHistory()
    {
        MoveHistory.Clear();
    }
}
