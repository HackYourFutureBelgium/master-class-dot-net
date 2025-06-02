namespace TicTacToe.Console;

class Program
{
    static void Main()
    {
        System.Console.Write("Enter name for Player 1 (X): ");
        var name1 = System.Console.ReadLine();
        System.Console.Write("Enter name for Player 2 (O): ");
        var name2 = System.Console.ReadLine();

        var p1 = new Player(name1, 'X');
        var p2 = new Player(name2, 'O');

        var playAgain = true;

        while (playAgain)
        {
            var boardSize = GetBoardSize();

            var game = new GameEngine(p1, p2, boardSize);
            game.Start();

            System.Console.WriteLine($"🏆 Scoreboard: {p1.Name} = {p1.Wins}, {p2.Name} = {p2.Wins}");

            playAgain = AskToPlayAgain();
        }
    }

    static int GetBoardSize()
    {
        while (true)
        {
            System.Console.Write("Choose board size (e.g., 3 for 3x3): ");
            var input = System.Console.ReadLine();

            if (int.TryParse(input, out var size) && size >= 3 && size <= 9)
                return size;

            System.Console.WriteLine("❌ Please enter a valid number between 3 and 9.");
        }
    }

    static bool AskToPlayAgain()
    {
        while (true)
        {
            System.Console.WriteLine("What would you like to do next?");
            System.Console.WriteLine("1. Play again");
            System.Console.WriteLine("2. Exit");
            var choice = System.Console.ReadLine();

            if (choice == "1")
                return true;
            if (choice == "2")
                return false;

            System.Console.WriteLine("❌ Please enter 1 or 2.");
        }
    }
}
