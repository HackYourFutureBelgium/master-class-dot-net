namespace TicTacToe.Console;

class Board
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

    public void Display()
    {
        System.Console.WriteLine();
        var count = 1;

        for (var i = 0; i < _size; i++)
        {
            for (var j = 0; j < _size; j++)
            {
                if (_cells[i, j] == '.')
                    System.Console.Write($"{count,3}");
                else
                    System.Console.Write($"{_cells[i, j],3}");

                if (j < _size - 1)
                    System.Console.Write(" |");

                count++;
            }

            System.Console.WriteLine();

            if (i < _size - 1)
                System.Console.WriteLine(new string('-', _size * 5));
        }

        System.Console.WriteLine();
    }

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

    public bool IsDraw()
    {
        foreach (var cell in _cells)
            if (cell == '.') return false;

        return true;
    }

    private (int, int) GetCoordinates(int position)
    {
        var row = (position - 1) / _size;
        var col = (position - 1) % _size;
        return (row, col);
    }

    public int MaxPosition() => _size * _size;
}
