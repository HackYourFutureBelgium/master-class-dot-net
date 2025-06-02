namespace TicTacToe.Console;

class GameController
{
    private Board board;
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private GameStatus status;

    public GameController(Player p1, Player p2, int boardSize)
    {
        board = new Board(boardSize);
        player1 = p1;
        player2 = p2;
        currentPlayer = player1;
        status = GameStatus.InProgress;
    }

    public void Start()
    {
        while (status == GameStatus.InProgress)
        {
            System.Console.Clear();
            board.Display();
            System.Console.Write($"{currentPlayer.Name} ({currentPlayer.Symbol}), enter a position (1-{board.MaxPosition()}): ");
            string input = System.Console.ReadLine();

            if (!int.TryParse(input, out int position) || position < 1 || position > board.MaxPosition())
            {
                System.Console.WriteLine("❌ Invalid input. Press Enter to try again.");
                System.Console.ReadLine();
                continue;
            }

            if (!board.IsMoveValid(position))
            {
                System.Console.WriteLine("🚫 Position already taken. Press Enter to try again.");
                System.Console.ReadLine();
                continue;
            }

            board.PlaceMove(position, currentPlayer.Symbol);

            if (board.CheckWin(currentPlayer.Symbol))
            {
                status = GameStatus.Win;
            }
            else if (board.IsDraw())
            {
                status = GameStatus.Draw;
            }
            else
            {
                SwitchPlayer();
            }
        }

        System.Console.Clear();
        board.Display();

        if (status == GameStatus.Win)
        {
            System.Console.WriteLine($"🎉 {currentPlayer.Name} wins!");
            currentPlayer.AddWin();
        }
        else
        {
            System.Console.WriteLine("🤝 It's a draw!");
        }
    }

    private void SwitchPlayer()
    {
        currentPlayer = currentPlayer == player1 ? player2 : player1;
    }
}
