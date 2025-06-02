namespace TicTacToe.Console;

class Program
{
    static void Main()
    {
        System.Console.Write("Enter name for Player 1 (X): ");
        string name1 = System.Console.ReadLine();
        System.Console.Write("Enter name for Player 2 (O): ");
        string name2 = System.Console.ReadLine();

        Player p1 = new Player(name1, 'X');
        Player p2 = new Player(name2, 'O');

        bool playAgain = true;

        while (playAgain)
        {
            int boardSize = GetBoardSize();

            var game = new GameController(p1, p2, boardSize);
            game.Start();

            System.Console.WriteLine($"🏆 Scoreboard: {p1.Name} = {p1.Wins}, {p2.Name} = {p2.Wins}");

            System.Console.WriteLine("What would you like to do next?");
            System.Console.WriteLine("1. Play again");
            System.Console.WriteLine("2. Exit");

            string choice = System.Console.ReadLine();
            playAgain = choice == "1";
        }
    }

    static int GetBoardSize()
    {
        while (true)
        {
            System.Console.Write("Choose board size (e.g., 3 for 3x3): ");
            string input = System.Console.ReadLine();

            if (int.TryParse(input, out int size) && size >= 3 && size <= 9)
                return size;

            System.Console.WriteLine("❌ Please enter a valid number between 3 and 9.");
        }
    }
}
