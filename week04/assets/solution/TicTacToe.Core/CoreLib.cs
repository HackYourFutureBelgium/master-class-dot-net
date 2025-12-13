using Microsoft.Extensions.Logging;

namespace TicTacToe.Core;

public class Board
{
    private readonly char[,] _cells;
    private readonly int _size;

    public Board(int size)
    {
        _size = size;
        _cells = new char[size, size];

        for (var i = 0; i < size; i++)
        for (var j = 0; j < size; j++)
            _cells[i, j] = '.';
    }

    public int Size => _size;

    public char GetCell(int row, int col) => _cells[row, col];

    public bool IsMoveValid(int position)
    {
        var (row, col) = GetCoordinates(position);
        return row < _size && col < _size && _cells[row, col] == '.';
    }

    public void PlaceMove(int position, char symbol)
    {
        var (row, col) = GetCoordinates(position);
        _cells[row, col] = symbol;
    }

    public bool CheckWin(char symbol)
    {
        const int winLength = 3;
        
        for (var i = 0; i < _size; i++)
        {
            for (var j = 0; j <= _size - winLength; j++)
            {
                if (Enumerable.Range(0, winLength).All(k => _cells[i, j + k] == symbol)) return true;
                if (Enumerable.Range(0, winLength).All(k => _cells[j + k, i] == symbol)) return true;
            }
        }
        for (var i = 0; i <= _size - winLength; i++)
        {
            for (var j = 0; j <= _size - winLength; j++)
            {
                if (Enumerable.Range(0, winLength).All(k => _cells[i + k, j + k] == symbol)) return true;
                if (Enumerable.Range(0, winLength).All(k => _cells[i + k, j + winLength - 1 - k] == symbol)) return true;
            }
        }

        return false;
    }

    public bool IsDraw() => _cells.Cast<char>().All(cell => cell != '.');

    private (int, int) GetCoordinates(int position)
    {
        var row = (position - 1) / _size;
        var col = (position - 1) % _size;
        return (row, col);
    }

    public void ClearCell(int row, int col)
    {
        _cells[row, col] = '.';
    }
}
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
public enum GameStatus
{
    InProgress,
    Win,
    Draw
}
public class Move
{
    public int Position { get; set; }
    public char Symbol { get; set; }
    public DateTime Timestamp { get; set; }
}
public class Player
{
    public string Name { get; }
    public char Symbol { get; }
    public int Wins { get; private set; }

    public Player(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
        Wins = 0;
    }

    public void AddWin() => Wins++;
}
