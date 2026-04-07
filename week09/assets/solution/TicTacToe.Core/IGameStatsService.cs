namespace TicTacToe.Core;

public interface IGameStatsService
{
    List<Move> MoveHistory { get; }
    List<Move> GlobalMoveHistory { get; }
    void AddMove(Move move);
    void ClearHistory();
}
