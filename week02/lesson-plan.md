# Lesson 2: Object-Oriented Programming & Improving the Game

## 📌 Lesson Overview
- Object-oriented programming in C#
- Encapsulation using access modifiers and properties
- **Interfaces** — defining contracts between classes
- Structuring the game with `Board`, `Player`, and `GameEngine` classes
- Game state management using enums
- Defensive programming and input validation
- Game loop for multiple rounds

---

## 1️⃣ Object-Oriented Programming Principles

Object-Oriented Programming (OOP) helps break down complex systems into smaller, reusable pieces. In this lesson, we apply these principles to structure our Tic-Tac-Toe game using the following concepts:

- **Encapsulation**: Keeping fields private and exposing public methods or properties to interact with them
- **Abstraction**: Hiding the inner workings of a class behind a simple interface
- **Single Responsibility**: Each class or method should do one thing and do it well

---

## 2️⃣ Interfaces

### What Is an Interface?

A class defines *what something is* and *how it works*. An interface defines only *what something can do* — a contract with no implementation.

Any class that signs that contract by implementing the interface must provide all the methods and properties it declares. The caller only needs to know about the contract, not which specific class is behind it.

```csharp
interface IGameRenderer
{
    void RenderBoard(char[,] cells);
    void RenderMessage(string message);
}
```

`IGameRenderer` says: "whatever you are, you must be able to render a board and a message." It says nothing about *how*.

### Implementing an Interface

A class opts in by listing the interface after a colon, then providing every member:

```csharp
class ConsoleRenderer : IGameRenderer
{
    public void RenderBoard(char[,] cells)
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
                Console.Write($" {cells[i, j]} ");
            Console.WriteLine();
        }
    }

    public void RenderMessage(string message)
    {
        Console.WriteLine(message);
    }
}
```

A second class can implement the same interface with completely different behaviour:

```csharp
class FileRenderer : IGameRenderer
{
    private readonly string _path;

    public FileRenderer(string path) => _path = path;

    public void RenderBoard(char[,] cells)    => File.AppendAllText(_path, BoardToString(cells));
    public void RenderMessage(string message) => File.AppendAllText(_path, message + "\n");

    private static string BoardToString(char[,] cells) { /* ... */ return ""; }
}
```

### Using an Interface as a Type

The key benefit: `GameEngine` can be written against `IGameRenderer` and work with *any* implementation — console, file, web, test fake — without changing a single line of game logic:

```csharp
class GameEngine
{
    private readonly IGameRenderer _renderer;

    public GameEngine(IGameRenderer renderer)
    {
        _renderer = renderer;
    }

    public void Start()
    {
        _renderer.RenderBoard(/* ... */);
        _renderer.RenderMessage("Your turn!");
    }
}

// Wire up the concrete choice once, in Main:
var game = new GameEngine(new ConsoleRenderer());
game.Start();
```

Swap `new ConsoleRenderer()` for `new FileRenderer("log.txt")` and the game logic stays untouched.

### Naming Convention

Interfaces are always prefixed with `I` by convention: `IGameRenderer`, `ILogger`, `IEnumerable`. When you see an `I`-prefixed type, you know it is a contract, not a concrete class.

### When to Reach for an Interface

Use an interface when:
- Multiple classes share a common set of behaviours but have different implementations
- You want to swap one implementation for another (e.g., real DB vs. in-memory in tests)
- You want to hide implementation details behind a clean API

> **Looking ahead:** From Lesson 3 onwards you will use `ILogger<T>` — a .NET interface for logging. You will never instantiate a logger directly; you receive one through the interface. In Lesson 9 you will mock `IGameStatsService` in tests, which only works because it is an interface. The pattern starts here.

---

## 3️⃣ The `Board` Class

The `Board` class handles board state, move placement, and display logic.

### 🎯 Responsibilities
- Represent the 3x3 grid
- Validate whether a move is legal
- Place a symbol on the board
- Print the current board

```csharp
class Board
{
    private readonly char[,] _cells;

    public Board()
    {
        _cells = new char[3, 3];
        var pos = 1;

        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            _cells[i, j] = pos++.ToString()[0];
    }

    public void Display()
    {
        Console.WriteLine();

        for (var i = 0; i < 3; i++)
        {
            Console.Write(" ");

            for (var j = 0; j < 3; j++)
            {
                Console.Write(_cells[i, j]);

                if (j < 2)
                    Console.Write(" | ");
            }

            Console.WriteLine();

            if (i < 2)
                Console.WriteLine("---|---|---");
        }

        Console.WriteLine();
    }

    public void PlaceMove(int position, char symbol)
    {
        var row = (position - 1) / 3;
        var col = (position - 1) % 3;
        _cells[row, col] = symbol;
    }

    public bool CheckWin(char symbol)
    {
        for (var i = 0; i < 3; i++)
        {
            if (_cells[i, 0] == symbol && _cells[i, 1] == symbol && _cells[i, 2] == symbol)
                return true;

            if (_cells[0, i] == symbol && _cells[1, i] == symbol && _cells[2, i] == symbol)
                return true;
        }

        if (_cells[0, 0] == symbol && _cells[1, 1] == symbol && _cells[2, 2] == symbol)
            return true;

        if (_cells[0, 2] == symbol && _cells[1, 1] == symbol && _cells[2, 0] == symbol)
            return true;

        return false;
    }

    public bool IsDraw()
    {
        foreach (var cell in _cells)
        {
            if (char.IsDigit(cell))
                return false;
        }

        return true;
    }
}
```

---

## 4️⃣ The `Player` Class

A player is represented by a name and a symbol. This class also prepares for score tracking later.

```csharp
class Player
{
    public string Name { get; }
    public char Symbol { get; }

    public Player(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
    }
}
```

---

## 5️⃣ Game Status Enumeration

Game states are expressed as an enum to clearly define and manage the status of the game.

```csharp
enum GameStatus
{
    InProgress,
    Win,
    Draw
}
```

---

## 6️⃣ The `GameEngine` Class

The `GameEngine` class handles turn switching, user input, and delegates board-related tasks.

```csharp
class GameEngine
{
    private readonly Board _board;
    private readonly Player _player1;
    private readonly Player _player2;
    private Player _currentPlayer;
    private GameStatus _status;

    public GameEngine(Player p1, Player p2)
    {
        _board = new Board();
        _player1 = p1;
        _player2 = p2;
        _currentPlayer = _player1;
        _status = GameStatus.InProgress;
    }

    public void Start()
    {
        while (_status == GameStatus.InProgress)
        {
            Console.Clear();
            _board.Display();

            Console.Write($"{_currentPlayer.Name} ({_currentPlayer.Symbol}), enter a position (1-9): ");
            var input = Console.ReadLine();
            var position = int.Parse(input);
            _board.PlaceMove(position, _currentPlayer.Symbol);

            if (_board.CheckWin(_currentPlayer.Symbol))
            {
                _status = GameStatus.Win;
            }
            else if (_board.IsDraw())
            {
                _status = GameStatus.Draw;
            }
            else
            {
                SwitchPlayer();
            }
        }

        Console.Clear();
        _board.Display();

        if (_status == GameStatus.Win)
            Console.WriteLine($"🎉 {_currentPlayer.Name} wins!");
        else if (_status == GameStatus.Draw)
            Console.WriteLine("🤝 It's a draw!");
    }

    private void SwitchPlayer()
    {
        _currentPlayer = _currentPlayer == _player1 ? _player2 : _player1;
    }
}
```

---

## 7️⃣ Playing the Game

This is the `Main` method, which initializes players and launches the controller.

```csharp
class Program
{
    static void Main()
    {
        Console.Write("Enter name for Player 1 (X): ");
        var name1 = Console.ReadLine();
        Console.Write("Enter name for Player 2 (O): ");
        var name2 = Console.ReadLine();

        var p1 = new Player(name1, 'X');
        var p2 = new Player(name2, 'O');

        var game = new GameEngine(p1, p2);
        game.Start();
    }
}
```

---

## 8️⃣ Defensive Programming & Input Validation

Defensive programming helps protect your application from unexpected input or states. We handle this using input validation and by checking for invalid moves.

- Always check if user input is a valid number
- Ensure position is in the valid range (1–9)
- Ensure the cell is not already occupied

### Example: Handling invalid move input

```csharp
if (!int.TryParse(input, out int position) || position < 1 || position > 9)
{
    Console.WriteLine("❌ Please enter a number between 1 and 9.");
    Console.ReadLine();
    continue;
}

if (!board.IsMoveValid(position))
{
    Console.WriteLine("🚫 That position is already taken.");
    Console.ReadLine();
    continue;
}
```

---

## 🚀 End of Lesson 2
