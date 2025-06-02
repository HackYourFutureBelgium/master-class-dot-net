namespace TicTacToe.Console;

class Board
{
    private char[,] cells;
    private int size;

    public Board(int size)
    {
        this.size = size;
        cells = new char[size, size];
        int pos = 1;

        for (int i = 0; i < size; i++)
        for (int j = 0; j < size; j++)
            cells[i, j] = '.';
    }

    public void Display()
    {
        System.Console.WriteLine();
        int count = 1;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (cells[i, j] == '.')
                    System.Console.Write($"{count,3}");
                else
                    System.Console.Write($"{cells[i, j],3}");

                if (j < size - 1)
                    System.Console.Write(" |");

                count++;
            }

            System.Console.WriteLine();

            if (i < size - 1)
                System.Console.WriteLine(new string('-', size * 5));
        }

        System.Console.WriteLine();
    }

    public bool IsMoveValid(int position)
    {
        (int row, int col) = GetCoordinates(position);
        return row < size && col < size && cells[row, col] == '.';
    }

    public void PlaceMove(int position, char symbol)
    {
        (int row, int col) = GetCoordinates(position);
        cells[row, col] = symbol;
    }

    public bool CheckWin(char symbol)
    {
        const int winLength = 3;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j <= size - winLength; j++)
            {
                if (Enumerable.Range(0, winLength).All(k => cells[i, j + k] == symbol)) return true;
                if (Enumerable.Range(0, winLength).All(k => cells[j + k, i] == symbol)) return true;
            }
        }

        for (int i = 0; i <= size - winLength; i++)
        {
            for (int j = 0; j <= size - winLength; j++)
            {
                if (Enumerable.Range(0, winLength).All(k => cells[i + k, j + k] == symbol)) return true;
                if (Enumerable.Range(0, winLength).All(k => cells[i + k, j + winLength - 1 - k] == symbol)) return true;
            }
        }

        return false;
    }

    public bool IsDraw()
    {
        foreach (char cell in cells)
            if (cell == '.') return false;

        return true;
    }

    private (int, int) GetCoordinates(int position)
    {
        int row = (position - 1) / size;
        int col = (position - 1) % size;
        return (row, col);
    }

    public int MaxPosition() => size * size;
}
