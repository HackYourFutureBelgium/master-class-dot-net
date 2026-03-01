# Lesson 9 – Homework: Achieving 100% Test Coverage

## 🎯 Homework Goal

The lesson walked through writing tests for `Board` and `GameEngine`. In this homework, you go further: **measure coverage and eliminate every untested branch** in `TicTacToe.Core`.

By the end, `dotnet test` reports 100% line and branch coverage for all Core classes.

---

## 📦 Step 1 – Create the Test Project

Create a new test project and wire it up:

```bash
dotnet new xunit -n TicTacToe.Tests
dotnet add TicTacToe.Tests reference TicTacToe.Core
dotnet add TicTacToe.Tests package Microsoft.Extensions.Logging.Abstractions
dotnet add TicTacToe.Tests package coverlet.collector
```

Verify everything compiles:

```bash
dotnet test
```

---

## 📋 Step 2 – Start from the Lesson Tests

From the lesson you have tests for:

- `NewBoard_IsAllEmpty`
- `IsMoveValid_OnEmptyCell_ReturnsTrue`
- `IsMoveValid_AfterPlacingMove_ReturnsFalse`
- `PlaceMove_SetsCorrectCell`
- `SetPlayers_FirstPlayerIsCurrentPlayer`
- `TryPlayMove_AfterValidMove_SwitchesPlayer`
- `TryPlayMove_WinningMove_SetsStatusToWin`
- `CheckWin_DetectsAllWinningCombinations` (the 8-row Theory)

Implement these first — they are your baseline. Then continue below.

---

## 📊 Step 3 – Measure Coverage

Run the tests and collect a coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

This produces a `coverage.cobertura.xml` file inside `TestResults/`. Install the report generator to read it as HTML:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
open coverage-report/index.html   # macOS
# or: start coverage-report/index.html  (Windows)
```

Open the report and look at each class in `TicTacToe.Core`. Red lines are untested. Your task is to turn them green.

---

## 🧪 Step 4 – Fill the Gaps

The lesson left several methods and branches completely untested. Work through them one by one.

---

### 4a – `Board.Size`

`Board` exposes a `Size` property. Write a test that creates a board of size 3 and asserts `Size` equals 3.

```csharp
[Fact]
public void Board_Size_ReturnsConstructorValue()
{
    // TODO
}
```

---

### 4b – `Board.IsDraw()`

The lesson tested draw detection through the engine. Test `IsDraw()` directly on a `Board` instance.

Write two tests:

```csharp
[Fact]
public void IsDraw_OnEmptyBoard_ReturnsFalse()
{
    // TODO
}

[Fact]
public void IsDraw_WhenAllCellsFilled_ReturnsTrue()
{
    // Hint: fill every cell with PlaceMove — you do not need a real game sequence
    // TODO
}
```

---

### 4c – `Board.ToFlatString()`

`ToFlatString()` returns the board as a single string reading left-to-right, top-to-bottom.

A new 3×3 board should return `"........."` (nine dots). After placing `'X'` at position 1, the first character should be `'X'`.

Write two tests for this method.

```csharp
[Fact]
public void ToFlatString_OnNewBoard_ReturnsAllDots()
{
    // TODO
}

[Fact]
public void ToFlatString_AfterPlacingMove_ReflectsSymbol()
{
    // TODO
}
```

---

### 4d – `GameStatsService`

`GameStatsService` has two lists and two methods. The lesson only called `AddMove` indirectly via the engine.

Write tests that target `GameStatsService` directly:

```csharp
[Fact]
public void AddMove_AppearsInBothHistoryLists()
{
    // Arrange: create a service, create a Move
    // Act: call AddMove
    // Assert: MoveHistory.Count and GlobalMoveHistory.Count are both 1
}

[Fact]
public void ClearHistory_RemovesFromMoveHistoryButNotGlobalHistory()
{
    // Arrange: add two moves, then call ClearHistory
    // Assert:
    //   - MoveHistory is empty
    //   - GlobalMoveHistory still has 2 entries
}
```

---

### 4e – `GameEngine.TryPlayMove` before setup

`TryPlayMove` has a null guard at the top:

```csharp
if (Board == null || _currentPlayer == null)
    return false;
```

This branch is never hit when you call `CreateEngine()`. Test it directly:

```csharp
[Fact]
public void TryPlayMove_WithoutSetup_ReturnsFalse()
{
    // Arrange: create a GameEngine with only a logger and stats service — no SetPlayers, no SetBoardSize
    // Act: call TryPlayMove(1)
    // Assert: result is false
}
```

---

### 4f – `GameEngine.TryUndoLastMove`

`TryUndoLastMove` has three distinct behaviors. Write one test for each:

**Behavior 1 – no moves to undo:**

```csharp
[Fact]
public void TryUndoLastMove_WithNoMoveHistory_ReturnsFalse()
{
    // Arrange: fresh engine, no moves played
    // Act + Assert: TryUndoLastMove returns false
}
```

**Behavior 2 – undo restores the board:**

```csharp
[Fact]
public void TryUndoLastMove_AfterOneMove_RestoresCellToEmpty()
{
    // Arrange: play one move at position 1
    // Act: call TryUndoLastMove
    // Assert:
    //   - returns true
    //   - GetCell(0, 0) on the board is '.' again
    //   - MoveHistory is empty
    //   - CurrentPlayer is back to Alice (the player who moved)
}
```

**Behavior 3 – undo resets a win:**

```csharp
[Fact]
public void TryUndoLastMove_AfterWinningMove_ResetsStatusToInProgress()
{
    // Arrange: play Alice's winning sequence, verify Status is Win
    // Act: call TryUndoLastMove once
    // Assert: Status is InProgress
}
```

---

## ✅ Step 5 – Reach 100%

Re-run the coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
open coverage-report/index.html
```

All classes in `TicTacToe.Core` should show 100% line coverage. If any lines are still red, read the report, find the uncovered branch, write the test, repeat.

---

## 🚫 Rules

- Do not test Blazor components or SignalR hubs
- All tests must pass: `dotnet test` shows no failures
- Do not copy test bodies from the lesson plan — write your own assertions

---

## ✅ What to Submit

- Your `TicTacToe.Tests` project
- A screenshot or copy of the HTML coverage report showing 100% for all Core classes
- All tests passing

---

## 🧠 Reminder

Coverage measures which lines ran — not whether the right thing happened. A test that calls a method but never asserts anything will show as covered. Make sure every test has a meaningful `Assert`.
