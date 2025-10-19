namespace TicTacToe.Console;

class Program
{
    static char[,] board = {
        { '1', '2', '3' },
        { '4', '5', '6' },
        { '7', '8', '9' }
    };

    static void Main(string[] args)
    {
        char currentPlayer = 'X';
        bool isGameRunning = true;

        System.Console.WriteLine("Welcome to 2D Tic Tac Toe! Type 'exit' to quit.");

        while (isGameRunning)
        {
            PrintBoard();
            System.Console.Write($"Player {currentPlayer}, enter a number (1-9): ");
            string input = System.Console.ReadLine();

            if (input?.ToLower() == "exit")
                break;

            if (!int.TryParse(input, out int move) || move < 1 || move > 9)
            {
                System.Console.WriteLine("Invalid input! Try again.");
                continue;
            }

            if (!TryMakeMove(move, currentPlayer))
            {
                System.Console.WriteLine("That cell is already taken!");
                continue;
            }

            if (IsBoardFull())
            {
                PrintBoard();
                System.Console.WriteLine("It's a draw!");
                break;
            }

            currentPlayer =
                currentPlayer == 'X'
                    ? 'O'
                    : 'X';
        }

        System.Console.WriteLine("Game Over.");
    }

    static void PrintBoard()
    {
        System.Console.WriteLine();

        for (int i = 0; i < 3; i++)
        {
            System.Console.WriteLine($" {board[i, 0]} | {board[i, 1]} | {board[i, 2]} ");

            if (i < 2)
                System.Console.WriteLine("---|---|---");
        }

        System.Console.WriteLine();
    }

    static bool TryMakeMove(int move, char player)
    {
        int row = (move - 1) / 3;
        int col = (move - 1) % 3;

        if (board[row, col] == 'X' || board[row, col] == 'O')
            return false;

        board[row, col] = player;
        return true;
    }

    static bool IsBoardFull()
    {
        foreach (char c in board)
        {
            if (c != 'X' && c != 'O')
                return false;
        }

        return true;
    }
}
