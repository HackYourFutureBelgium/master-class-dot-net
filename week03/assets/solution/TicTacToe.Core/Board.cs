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
