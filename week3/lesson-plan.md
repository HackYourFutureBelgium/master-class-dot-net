# Lesson 3: Collections, LINQ & Structuring Code

## 📌 Lesson Overview
- Collections in C#
- Introduction to LINQ
- Project structure and code reuse across assemblies
- Extracting reusable game logic into a class library
- Singleton pattern (classic and DI-based)
- Basic logging using try-catch

---

## 1️⃣ Working with Collections in C#

### Lists

```csharp
List<string> moves = new List<string> { "X", "O", "X" };
moves.Add("O");
```

### LINQ

```csharp
var xMoves = moves.Where(m => m == "X").Count();
```

LINQ makes it easier to filter, sort, and project data from collections.

---

## 2️⃣ Extracting Logic into a Class Library

Refactor game logic out of the console app and into a new project:
📁 `TicTacToe.Core` (Class Library)

```bash
dotnet new classlib -n TicTacToe.Core
```

In `TicTacToe.Core/GameEngine.cs`:

```csharp
namespace TicTacToe.Core;

public class GameEngine
{
    private char[] board;
    private char currentPlayer;

    public GameEngine()
    {
        board = Enumerable.Range(1, 9).Select(i => i.ToString()[0]).ToArray();
        currentPlayer = 'X';
    }

    public char[] Board => board;

    public char CurrentPlayer => currentPlayer;

    public bool IsMoveValid(int pos) =>
        pos >= 1 && pos <= 9 && char.IsDigit(board[pos - 1]);

    public void PlaceMove(int pos)
    {
        board[pos - 1] = currentPlayer;
    }

    public bool CheckWin()
    {
        int[,] winCombos = new int[,]
        {
            {0,1,2}, {3,4,5}, {6,7,8},
            {0,3,6}, {1,4,7}, {2,5,8},
            {0,4,8}, {2,4,6}
        };

        for (int i = 0; i < winCombos.GetLength(0); i++)
        {
            if (board[winCombos[i, 0]] == currentPlayer &&
                board[winCombos[i, 1]] == currentPlayer &&
                board[winCombos[i, 2]] == currentPlayer)
                return true;
        }

        return false;
    }

    public bool IsDraw() => board.All(c => !char.IsDigit(c));

    public void SwitchPlayer()
    {
        currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
    }
}
```

Then reference the class library from your console project:

```bash
dotnet add reference ../TicTacToe.Core/TicTacToe.Core.csproj
```

---

## 3️⃣ Using the GameEngine in Console App

```csharp
using TicTacToe.Core;

var game = new GameEngine();

while (true)
{
    Console.Clear();
    PrintBoard(game.Board);
    Console.Write($"Player {game.CurrentPlayer}, enter move: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out int move) && game.IsMoveValid(move))
    {
        game.PlaceMove(move);

        if (game.CheckWin())
        {
            Console.Clear();
            PrintBoard(game.Board);
            Console.WriteLine($"🎉 Player {game.CurrentPlayer} wins!");
            break;
        }
        if (game.IsDraw())
        {
            Console.Clear();
            PrintBoard(game.Board);
            Console.WriteLine("🤝 It's a draw!");
            break;
        }

        game.SwitchPlayer();
    }
    else
    {
        Console.WriteLine("❌ Invalid move.");
        Console.ReadLine();
    }
}

void PrintBoard(char[] b)
{
    for (int i = 0; i < 9; i += 3)
    {
        Console.WriteLine($" {b[i]} | {b[i+1]} | {b[i+2]} ");
        if (i < 6) Console.WriteLine("---|---|---");
    }
}
```

---

## 4️⃣ Singleton Pattern

### Classical Singleton

```csharp
public class GameLogger
{
    private static GameLogger? _instance;

    private GameLogger() {}

    public static GameLogger Instance => _instance ??= new GameLogger();

    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}
```

Usage:

```csharp
GameLogger.Instance.Log("Move placed.");
```

---

### Singleton with Dependency Injection

Add this to a console app using the Generic Host:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<GameLogger>();
    })
    .Build();

var logger = host.Services.GetRequiredService<GameLogger>();
logger.Log("This logger is injected!");
```

You can now use `GameLogger` in any class via constructor injection.

---

## 5️⃣ Basic Error Handling with Logging

```csharp
try
{
    int input = Convert.ToInt32(Console.ReadLine());
}
catch (Exception ex)
{
    GameLogger.Instance.Log($"❌ Invalid input: {ex.Message}");
}
```

---

## 🚀 End of Lesson 3