using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TicTacToe.Core;

class Program
{
    private static GameEngine _engine;

    static void Main()
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
                {
                    services.AddSingleton<GameStatsService>();
                    services.AddSingleton<GameEngine>();
                }
            )
            .Build();

        _engine = host.Services.GetRequiredService<GameEngine>();

        Console.Write("Enter name for Player 1 (X): ");
        var name1 = Console.ReadLine();
        Console.Write("Enter name for Player 2 (O): ");
        var name2 = Console.ReadLine();

        var p1 = new Player(name1, 'X');
        var p2 = new Player(name2, 'O');

        _engine.SetPlayers(p1, p2);

        var playAgain = true;

        while (playAgain)
        {
            var boardSize = GetBoardSize();
            _engine.SetBoardSize(boardSize);
            _engine.History.ClearHistory();

            while (_engine.Status == GameStatus.InProgress)
            {
                Console.Clear();
                PrintBoard(_engine.Board);
                Console.WriteLine("Type a position number, or 'undo' to undo last move.");
                Console.Write($"{_engine.CurrentPlayer.Name} ({_engine.CurrentPlayer.Symbol}), enter position: ");
                var input = Console.ReadLine();

                if (input == "undo")
                {
                    if (_engine.TryUndoLastMove())
                    {
                        Console.WriteLine("✅ Last move undone.");
                    }
                    else
                    {
                        Console.WriteLine("❌ Nothing to undo.");
                    }

                    Console.ReadLine();
                    continue;
                }

                if (!int.TryParse(input, out var position) || !_engine.TryPlayMove(position))
                {
                    Console.WriteLine("❌ Invalid move. Press Enter to try again.");
                    Console.ReadLine();
                }
            }

            Console.Clear();
            PrintBoard(_engine.Board);
            Console.WriteLine(
                _engine.Status == GameStatus.Win
                    ? $"🎉 {_engine.CurrentPlayer.Name} wins!"
                    : "🤝 It's a draw!"
            );
            ListMoves();
            Console.WriteLine($"🏆 Scoreboard: {p1.Name} = {p1.Wins}, {p2.Name} = {p2.Wins}");
            DisplayPositionStats();
            ExportMovesToFile(_engine.History.MoveHistory);
            playAgain = AskToPlayAgain(_engine.History.MoveHistory, boardSize);
        }
    }

    static int GetBoardSize()
    {
        while (true)
        {
            Console.Write("Choose board size (e.g., 3 for 3x3): ");
            var input = Console.ReadLine();

            if (int.TryParse(input, out var size) && size >= 3 && size <= 9)
                return size;

            Console.WriteLine("❌ Please enter a valid number between 3 and 9.");
        }
    }

    static bool AskToPlayAgain(List<Move> moves, int boardSize)
    {
        while (true)
        {
            Console.WriteLine("What would you like to do next?");
            Console.WriteLine("1. Play again");
            Console.WriteLine("2. Replay this game");
            Console.WriteLine("3. Exit");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    return true;

                case "2":
                    ReplayGame(moves, boardSize);
                    // After replay, re-show the menu
                    continue;

                case "3":
                    return false;

                default:
                    Console.WriteLine("❌ Please enter 1, 2, or 3.");
                    break;
            }
        }
    }

    static void PrintBoard(Board board)
    {
        Console.WriteLine();
        var count = 1;
        for (var i = 0; i < board.Size; i++)
        {
            for (var j = 0; j < board.Size; j++)
            {
                var symbol = board.GetCell(i, j);

                if (symbol == '.')
                    Console.Write($"{count,3}");
                else
                    Console.Write($"{symbol,3}");

                if (j < board.Size - 1)
                    Console.Write(" |");

                count++;
            }

            Console.WriteLine();

            if (i < board.Size - 1)
                Console.WriteLine(new string('-', board.Size * 5));
        }

        Console.WriteLine();
    }

    static void ListMoves()
    {
        var ordered = _engine.History.MoveHistory.OrderBy(m => m.Timestamp);
        Console.WriteLine("Move history:");

        foreach (var move in ordered)
        {
            Console.WriteLine($"Player {move.Symbol} played at {move.Position} on {move.Timestamp}");
        }
    }

    static void DisplayPositionStats()
    {
        var grouped = _engine.History.GlobalMoveHistory
            .GroupBy(m => m.Position)
            .OrderBy(g => g.Key)
            .Select(g => new { Position = g.Key, Count = g.Count() });

        Console.WriteLine("Position usage across all rounds:");

        foreach (var stat in grouped)
        {
            Console.WriteLine($"Position {stat.Position} was played {stat.Count} time(s)");
        }
    }

    static void ExportMovesToFile(List<Move> moves)
    {
        var filename = $"moves_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var lines = moves.Select(m => $"{m.Timestamp:HH:mm:ss} - {m.Symbol} played {m.Position}");
        File.WriteAllLines(filename, lines);
        Console.WriteLine($"📄 Moves exported to {filename}");
    }

    static void ReplayGame(List<Move> moves, int boardSize)
    {
        var board = new Board(boardSize);
        Console.Clear();

        foreach (var move in moves.OrderBy(m => m.Timestamp))
        {
            board.PlaceMove(move.Position, move.Symbol);
            PrintBoard(board);
            Console.WriteLine($"Player {move.Symbol} played at {move.Position}");
            Thread.Sleep(1000); // 1-second delay
            Console.Clear();
        }

        Console.WriteLine("🎬 Replay complete. Press Enter to return to menu.");
        Console.ReadLine();
    }
}