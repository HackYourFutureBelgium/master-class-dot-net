using Microsoft.Extensions.Logging;

namespace TicTacToe.Core;

public class GameEngine
{
    private readonly ILogger<GameEngine> _logger;
    private Board? _board;
    private Player? _player1;
    private Player? _player2;
    private Player? _currentPlayer;
    private GameStatus _status;

    public GameStatsService History { get; }
    
    public GameEngine(ILogger<GameEngine> logger, GameStatsService historyService)
    {
        _logger = logger;
        History = historyService;
    }

    public void SetPlayers(Player p1, Player p2)
    {
        _player1 = p1;
        _player2 = p2;
        _currentPlayer = _player1;
    }

    public void SetBoardSize(int size)
    {
        _board = new Board(size);
        _status = GameStatus.InProgress;
    }

    public GameStatus Status => _status;

    public Player CurrentPlayer => _currentPlayer!;

    public Board Board => _board!;

    public bool TryPlayMove(int position)
    {
        if (_board == null || _currentPlayer == null)
            return false;

        if (!_board.IsMoveValid(position))
        {
            _logger.LogWarning("Invalid move by {PlayerName} at position {Position}.", _currentPlayer.Name, position);
            return false;
        }

        _board.PlaceMove(position, _currentPlayer.Symbol);
        History.AddMove(new Move { Position = position, Symbol = _currentPlayer.Symbol, Timestamp = DateTime.Now });

        if (_board.CheckWin(_currentPlayer.Symbol))
        {
            _status = GameStatus.Win;
            _currentPlayer.AddWin();
        }
        else if (_board.IsDraw())
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
        _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
    }
}
