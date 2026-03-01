# Lesson 9: Unit Testing with xUnit

## 📌 Lesson Overview

In this lesson, we introduce **unit testing** using **xUnit**, the most commonly used testing framework in modern .NET projects.

You will learn:
- What unit tests are and why they matter for the game you built
- How to create a test project and reference Core
- How xUnit works: `[Fact]`, `[Theory]`, and constructor setup
- How to write meaningful tests for `Board` and `GameEngine`
- How to handle constructor dependencies using `NullLogger<T>`
- How to test all eight winning combinations with a single `[Theory]`

---

## 🧭 Lesson Flow (Recommended)

1. Why tests matter — a motivating moment from our game
2. What unit testing is and what xUnit provides
3. Creating the test project and adding the reference
4. Anatomy of a test — Arrange / Act / Assert
5. Shared setup with the class constructor
6. Testing `Board` logic in depth
7. Handling dependencies: `NullLogger<T>` for `GameEngine`
8. Testing `GameEngine` logic
9. Parameterized tests: `[Theory]` for all eight winning combinations
10. Common assertions reference
11. `Assert.Throws` and when our game uses it
12. Naming tests well
13. What to test and what not to

---

## 1️⃣ Why Tests Matter for Our Game

Consider the game you built in lessons 7 and 8. `CheckWin` is a non-trivial method: it checks three rows, three columns, and two diagonals. Without tests, you have to mentally trace through the logic or run the entire app and play through a game to verify it works.

With tests, you can verify all eight winning combinations in under a second, every time you change something.

Here is the motivating question:

> **What if you wanted to refactor `CheckWin` to be more efficient — could you do it safely?**

Without tests: no. You would have to manually verify every scenario in the browser.

With tests: yes. Run `dotnet test`. If all tests pass, you know nothing is broken.

This is why tests exist: **confidence when changing code**.

```
Board.CheckWin()

Without tests                With tests
─────────────────────────    ──────────────────────────────
Run the app                  dotnet test
Click through a game         8 scenarios verified in <1s
"Seems to work..."           Green ✅ or specific failure ❌
Repeat for every scenario    Repeat in milliseconds
```

---

## 2️⃣ What Is Unit Testing?

A **unit test** is a small piece of code that verifies the behavior of a **single unit** of logic.

A unit is typically:
- A method
- A class
- A small, isolated piece of functionality

Unit tests should be:
- Fast
- Deterministic (same result every time)
- Independent (no shared state between tests)
- Easy to understand

They help you:
- Detect bugs early
- Refactor with confidence
- Document expected behavior
- Avoid regressions

---

## 3️⃣ What Is xUnit?

**xUnit** is the standard unit testing framework for modern .NET.

It is open source, widely adopted, and designed around modern C# practices.

Key design choices:
- `[Fact]` marks a single test method
- `[Theory]` marks a parameterized test (same logic, multiple inputs)
- **Constructor injection** is used for shared setup — no `[SetUp]` attribute
- `Assert` methods express expected behavior explicitly

---

## 4️⃣ Solution Structure

We add a **third project** for tests:

```
/TicTacToe.Solution
  /TicTacToe.Core        ← Game logic
  /TicTacToe.Web         ← Blazor Server
  /TicTacToe.Tests       ← Unit tests (xUnit)
```

Key rules:
- Tests reference **Core**
- Tests do **not** reference Web
- UI is not unit-tested

---

## 5️⃣ Creating a Test Project

Create a new test project:

```bash
dotnet new xunit -n TicTacToe.Tests
```

Add a reference to the Core project:

```bash
dotnet add TicTacToe.Tests reference TicTacToe.Core
```

Add the logging abstractions package (needed to create a `NullLogger` in section 9):

```bash
dotnet add TicTacToe.Tests package Microsoft.Extensions.Logging.Abstractions
```

Run all tests:

```bash
dotnet test
```

---

## 6️⃣ Anatomy of an xUnit Test

Every test follows the **Arrange / Act / Assert** pattern.

```csharp
using TicTacToe.Core;

public class BoardTests
{
    [Fact]
    public void NewBoard_IsAllEmpty()
    {
        // Arrange
        var board = new Board(3);

        // Act — nothing to do; we are inspecting initial state

        // Assert
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            Assert.Equal('.', board.GetCell(i, j));
    }
}
```

Key concepts:
- `[Fact]` marks a single test
- Test methods return `void`
- The three sections keep tests readable and structured

---

## 7️⃣ Shared Setup with the xUnit Constructor

When several tests in the same class need the same starting state, put the setup in the **constructor**.

xUnit creates a **new instance** of the test class before every test. This guarantees tests are completely isolated — no shared state can leak between them.

```csharp
public class BoardTests
{
    private readonly Board _board;

    public BoardTests()
    {
        // Runs before every test in this class
        _board = new Board(3);
    }

    [Fact]
    public void NewBoard_IsAllEmpty()
    {
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            Assert.Equal('.', _board.GetCell(i, j));
    }

    [Fact]
    public void IsMoveValid_OnEmptyBoard_ReturnsTrue()
    {
        Assert.True(_board.IsMoveValid(1));
    }
}
```

> There is no `[SetUp]` attribute in xUnit. The constructor is the setup.

---

## 8️⃣ Testing Board Logic

Here is a complete test class covering the `Board`'s core responsibilities.

Position mapping on a 3×3 board (positions are 1-indexed):

```
1 | 2 | 3      (row 0)
4 | 5 | 6      (row 1)
7 | 8 | 9      (row 2)
```

```csharp
using TicTacToe.Core;

public class BoardTests
{
    private readonly Board _board;

    public BoardTests()
    {
        _board = new Board(3);
    }

    [Fact]
    public void NewBoard_IsAllEmpty()
    {
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            Assert.Equal('.', _board.GetCell(i, j));
    }

    [Fact]
    public void IsMoveValid_OnEmptyCell_ReturnsTrue()
    {
        Assert.True(_board.IsMoveValid(1));
    }

    [Fact]
    public void IsMoveValid_AfterPlacingMove_ReturnsFalse()
    {
        _board.PlaceMove(1, 'X');

        Assert.False(_board.IsMoveValid(1));
    }

    [Fact]
    public void PlaceMove_SetsCorrectCell()
    {
        _board.PlaceMove(5, 'X');

        // Position 5 maps to row 1, column 1 on a 3x3 board
        Assert.Equal('X', _board.GetCell(1, 1));
    }

    [Fact]
    public void CheckWin_WithNoMoves_ReturnsFalse()
    {
        Assert.False(_board.CheckWin('X'));
    }

    [Fact]
    public void IsDraw_OnEmptyBoard_ReturnsFalse()
    {
        Assert.False(_board.IsDraw());
    }
}
```

Each test covers exactly one behavior. If a test fails, you know immediately what broke without reading the others.

---

## 9️⃣ Dependencies in Tests: NullLogger

`GameEngine` requires an `ILogger<GameEngine>` in its constructor:

```csharp
public GameEngine(ILogger<GameEngine> logger, GameStatsService historyService)
```

In production, the ASP.NET Core DI container provides the logger automatically. In a test project there is no DI container — you have to provide the dependency yourself.

The solution is `NullLogger<T>`: a do-nothing logger provided by .NET specifically for this situation. It accepts all log calls and silently discards them.

```csharp
using Microsoft.Extensions.Logging.Abstractions;

var logger = NullLogger<GameEngine>.Instance;
var engine = new GameEngine(logger, new GameStatsService());
```

Because creating a ready-to-use engine always requires the same three steps, extract a **private helper method**. This keeps every test short and avoids repetition:

```csharp
public class GameEngineTests
{
    private GameEngine CreateEngine()
    {
        var engine = new GameEngine(
            NullLogger<GameEngine>.Instance,
            new GameStatsService());

        engine.SetPlayers(
            new Player("Alice", 'X'),
            new Player("Bob", 'O'));

        engine.SetBoardSize(3);

        return engine;
    }
}
```

Every test in this class calls `CreateEngine()` to get a fresh, independent engine with no move history.

> **Why not put the engine in the constructor?**
> Either approach works. A helper method is useful when tests need to make slightly different setup choices (different board size, different players). A constructor field is cleaner when every test needs exactly the same starting state.

---

## 🔟 Testing GameEngine Logic

```csharp
using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Core;

public class GameEngineTests
{
    private GameEngine CreateEngine()
    {
        var engine = new GameEngine(
            NullLogger<GameEngine>.Instance,
            new GameStatsService());

        engine.SetPlayers(
            new Player("Alice", 'X'),
            new Player("Bob", 'O'));

        engine.SetBoardSize(3);

        return engine;
    }

    [Fact]
    public void SetPlayers_FirstPlayerIsCurrentPlayer()
    {
        var engine = CreateEngine();

        Assert.Equal("Alice", engine.CurrentPlayer.Name);
    }

    [Fact]
    public void TryPlayMove_ValidMove_ReturnsTrue()
    {
        var engine = CreateEngine();

        var result = engine.TryPlayMove(1);

        Assert.True(result);
    }

    [Fact]
    public void TryPlayMove_AfterValidMove_SwitchesPlayer()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice plays

        Assert.Equal("Bob", engine.CurrentPlayer.Name);
    }

    [Fact]
    public void TryPlayMove_OnOccupiedCell_ReturnsFalse()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // Alice plays at 1
        var result = engine.TryPlayMove(1); // Bob tries the same cell

        Assert.False(result);
    }

    [Fact]
    public void TryPlayMove_WinningMove_SetsStatusToWin()
    {
        var engine = CreateEngine();

        // X wins the top row: positions 1, 2, 3
        // O plays 4 and 5 in between
        engine.TryPlayMove(1); // X
        engine.TryPlayMove(4); // O
        engine.TryPlayMove(2); // X
        engine.TryPlayMove(5); // O
        engine.TryPlayMove(3); // X — top row complete

        Assert.Equal(GameStatus.Win, engine.Status);
    }

    [Fact]
    public void TryPlayMove_WinningMove_IncrementsWinnerWins()
    {
        var engine = CreateEngine();

        engine.TryPlayMove(1); // X
        engine.TryPlayMove(4); // O
        engine.TryPlayMove(2); // X
        engine.TryPlayMove(5); // O
        engine.TryPlayMove(3); // X wins

        Assert.Equal(1, engine.Player1.Wins);
    }

    [Fact]
    public void TryPlayMove_DrawSequence_SetsStatusToDraw()
    {
        var engine = CreateEngine();

        // Fills the board without a winner:
        // X | O | X
        // O | X | X
        // O | X | O
        engine.TryPlayMove(1); // X
        engine.TryPlayMove(2); // O
        engine.TryPlayMove(3); // X
        engine.TryPlayMove(4); // O
        engine.TryPlayMove(5); // X
        engine.TryPlayMove(9); // O
        engine.TryPlayMove(6); // X
        engine.TryPlayMove(7); // O
        engine.TryPlayMove(8); // X — board full, no winner

        Assert.Equal(GameStatus.Draw, engine.Status);
    }
}
```

---

## 1️⃣1️⃣ Parameterized Tests with `[Theory]`

When the same behavior must be verified for multiple inputs, use `[Theory]` with `[InlineData]`.

A 3×3 board has exactly **eight** winning combinations: three rows, three columns, and two diagonals. Instead of writing eight nearly identical tests, write one parameterized test:

```csharp
[Theory]
[InlineData(1, 2, 3)] // row 0
[InlineData(4, 5, 6)] // row 1
[InlineData(7, 8, 9)] // row 2
[InlineData(1, 4, 7)] // column 0
[InlineData(2, 5, 8)] // column 1
[InlineData(3, 6, 9)] // column 2
[InlineData(1, 5, 9)] // diagonal ↘
[InlineData(3, 5, 7)] // diagonal ↙
public void CheckWin_DetectsAllWinningCombinations(int pos1, int pos2, int pos3)
{
    var board = new Board(3);

    board.PlaceMove(pos1, 'X');
    board.PlaceMove(pos2, 'X');
    board.PlaceMove(pos3, 'X');

    Assert.True(board.CheckWin('X'));
}
```

xUnit runs this test once per `[InlineData]` row — eight independent test runs from a single method. If only the diagonal logic is broken, exactly those two rows fail. You know immediately where to look.

---

## 1️⃣2️⃣ Common xUnit Assertions

### Assert.Equal / Assert.NotEqual

```csharp
Assert.Equal(GameStatus.Win, engine.Status);
Assert.NotEqual(0, engine.Player1.Wins);
Assert.Equal("Alice", engine.CurrentPlayer.Name);
```

---

### Assert.True / Assert.False

```csharp
Assert.True(board.IsMoveValid(1));
Assert.False(board.IsMoveValid(1)); // after the cell is occupied
Assert.True(engine.TryPlayMove(1));
```

---

### Assert.Null / Assert.NotNull

```csharp
Assert.NotNull(engine.CurrentPlayer);
```

---

### Assert.Empty / Assert.NotEmpty

```csharp
Assert.Empty(engine.History.MoveHistory);    // fresh engine, no moves
Assert.NotEmpty(engine.History.MoveHistory); // after at least one move
```

---

### Assert.Contains / Assert.DoesNotContain

```csharp
Assert.Contains("Alice", playerNames);
```

---

## 1️⃣3️⃣ Assert.Throws

Use `Assert.Throws` when a method is expected to throw an exception:

```csharp
Assert.Throws<InvalidOperationException>(() =>
{
    service.DoSomethingInvalid();
});
```

**Note for our game:** the logic in `TicTacToe.Core` uses return values rather than exceptions to communicate invalid state. `TryPlayMove` returns `false` for an invalid move — it does not throw. In this codebase, you will use `Assert.False` and `Assert.Equal` to verify those return values. `Assert.Throws` becomes relevant when you design APIs that signal failure by throwing.

---

## 1️⃣4️⃣ Naming Tests Properly

A good test name reads like a specification:

```
MethodName_Scenario_ExpectedResult
```

Examples:

```csharp
// ✅ Clear
IsMoveValid_AfterPlacingMove_ReturnsFalse
TryPlayMove_WinningMove_SetsStatusToWin
CheckWin_WithNoMoves_ReturnsFalse

// ❌ Vague
Test1
BoardTest
CheckWinWorks
```

A failing test with a good name tells you exactly what broke without reading the test body.

---

## 1️⃣5️⃣ What to Test and What Not To

### ✅ Good candidates
- Game rules (`CheckWin`, `IsDraw`)
- Validation logic (`IsMoveValid`, `TryPlayMove` return values)
- State transitions (player switching, `GameStatus` changes)
- Move history (`GameStatsService`)

### ❌ Avoid unit testing
- UI rendering (Blazor components)
- SignalR connections and hubs
- HTML output

The rule is simple: if it lives in `TicTacToe.Core`, it is a candidate for unit testing.

---

## ✅ Wrap-Up

By the end of this lesson, you can:
- Create an xUnit test project and reference Core
- Write clean and meaningful tests using Arrange / Act / Assert
- Share setup across tests using the class constructor
- Handle the `ILogger` dependency with `NullLogger<T>`
- Test all game behaviors: empty board, valid/invalid moves, player switching, win, and draw
- Use `[Theory]` to verify all eight winning combinations with a single test method
