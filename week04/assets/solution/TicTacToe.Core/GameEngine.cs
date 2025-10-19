using Microsoft.Extensions.Logging;

namespace TicTacToe.Core;

public class GameEngine
{
    private readonly ILogger<GameEngine> _logger;
    private Player _currentPlayer;
    private GameStatus _status;

    public Board Board { get; private set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentPlayer => _currentPlayer!;
    public GameStatus Status => _status;
    public GameStatsService History { get; }

    public GameEngine(ILogger<GameEngine> logger, GameStatsService historyService)
    {
        _logger = logger;
        History = historyService;
    }

    public void SetPlayers(Player p1, Player p2)
    {
        Player1 = p1;
        Player2 = p2;
        _currentPlayer = Player1;
    }

    public void SetBoardSize(int size)
    {
        Board = new Board(size);
        _status = GameStatus.InProgress;
    }

    public bool TryPlayMove(int position)
    {
        if (Board == null || _currentPlayer == null)
            return false;

        if (!Board.IsMoveValid(position))
        {
            _logger.LogWarning("Invalid move by {PlayerName} at position {Position}.", _currentPlayer.Name, position);
            return false;
        }

        Board.PlaceMove(position, _currentPlayer.Symbol);
        History.AddMove(new Move { Position = position, Symbol = _currentPlayer.Symbol, Timestamp = DateTime.Now });

        if (Board.CheckWin(_currentPlayer.Symbol))
        {
            _status = GameStatus.Win;
            _currentPlayer.AddWin();
        }
        else if (Board.IsDraw())
        {
            _status = GameStatus.Draw;
        }
        else
        {
            SwitchPlayer();
        }

        return true;
    }

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == Player1 ? Player2 : Player1;
    }

    public bool TryUndoLastMove()
    {
        if (History.MoveHistory.Count == 0 || Board == null)
            return false;

        var lastMove = History.MoveHistory.Last();
        var (row, col) = GetCoordinates(lastMove.Position);
        Board.ClearCell(row, col);
        History.MoveHistory.RemoveAt(History.MoveHistory.Count - 1);
        SwitchPlayer();
        _status = GameStatus.InProgress;
        return true;
    }

    private (int, int) GetCoordinates(int position)
    {
        var row = (position - 1) / Board!.Size;
        var col = (position - 1) % Board!.Size;
        return (row, col);
    }
}
