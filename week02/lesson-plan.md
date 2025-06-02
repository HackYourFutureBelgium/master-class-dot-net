# Lesson 2: Object-Oriented Programming & Improving the Game

## 📌 Lesson Overview
- Object-oriented programming in C#
- Encapsulation using access modifiers and properties
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

## 2️⃣ The `Board` Class

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

## 3️⃣ The `Player` Class

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

## 4️⃣ Game Status Enumeration

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

## 5️⃣ The `GameEngine` Class

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

## 6️⃣ Playing the Game

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

## 7️⃣ Defensive Programming & Input Validation

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
