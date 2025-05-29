# Lesson 2: Object-Oriented Programming & Improving the Game

## 📌 Lesson Overview
- Object-oriented programming in C#
- Encapsulation using access modifiers and properties
- Structuring the game with `Board`, `Player`, and `GameController` classes
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
    private char[,] cells;

    public Board()
    {
        cells = new char[3, 3];
        int pos = 1;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                cells[i, j] = pos++.ToString()[0];
    }

    public void Display()
    {
        Console.WriteLine();
        for (int i = 0; i < 3; i++)
        {
            Console.Write(" ");
            for (int j = 0; j < 3; j++)
            {
                Console.Write(cells[i, j]);
                if (j < 2) Console.Write(" | ");
            }
            Console.WriteLine();
            if (i < 2) Console.WriteLine("---|---|---");
        }
        Console.WriteLine();
    }

    public bool IsMoveValid(int position)
    {
        int row = (position - 1) / 3;
        int col = (position - 1) % 3;
        return char.IsDigit(cells[row, col]);
    }

    public void PlaceMove(int position, char symbol)
    {
        int row = (position - 1) / 3;
        int col = (position - 1) % 3;
        cells[row, col] = symbol;
    }

    public bool CheckWin(char symbol)
    {
        for (int i = 0; i < 3; i++)
        {
            if (cells[i, 0] == symbol && cells[i, 1] == symbol && cells[i, 2] == symbol) return true;
            if (cells[0, i] == symbol && cells[1, i] == symbol && cells[2, i] == symbol) return true;
        }
        if (cells[0, 0] == symbol && cells[1, 1] == symbol && cells[2, 2] == symbol) return true;
        if (cells[0, 2] == symbol && cells[1, 1] == symbol && cells[2, 0] == symbol) return true;

        return false;
    }

    public bool IsDraw()
    {
        foreach (char cell in cells)
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

## 5️⃣ The `GameController` Class

The `GameController` class handles turn switching, user input, and delegates board-related tasks.

```csharp
class GameController
{
    private Board board;
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private GameStatus status;

    public GameController(Player p1, Player p2)
    {
        board = new Board();
        player1 = p1;
        player2 = p2;
        currentPlayer = player1;
        status = GameStatus.InProgress;
    }

    public void Start()
    {
        while (status == GameStatus.InProgress)
        {
            Console.Clear();
            board.Display();

            Console.Write($"{currentPlayer.Name} ({currentPlayer.Symbol}), enter a position (1-9): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int position) && position >= 1 && position <= 9)
            {
                if (!board.IsMoveValid(position))
                {
                    Console.WriteLine("🚫 Position already taken. Press Enter to try again.");
                    Console.ReadLine();
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
            else
            {
                Console.WriteLine("❌ Invalid input. Press Enter to try again.");
                Console.ReadLine();
            }
        }

        Console.Clear();
        board.Display();
        if (status == GameStatus.Win)
            Console.WriteLine($"🎉 {currentPlayer.Name} wins!");
        else if (status == GameStatus.Draw)
            Console.WriteLine("🤝 It's a draw!");
    }

    private void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == player1) ? player2 : player1;
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
        string name1 = Console.ReadLine();
        Console.Write("Enter name for Player 2 (O): ");
        string name2 = Console.ReadLine();

        Player p1 = new Player(name1, 'X');
        Player p2 = new Player(name2, 'O');

        bool playAgain = true;
        while (playAgain)
        {
            GameController game = new GameController(p1, p2);
            game.Start();

            Console.Write("Do you want to play again? (y/n): ");
            string answer = Console.ReadLine().ToLower();
            playAgain = answer == "y";
        }
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
