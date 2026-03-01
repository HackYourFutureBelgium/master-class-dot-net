# Lesson 9 – Preparation: Unit Testing with xUnit

## 🎯 Goal

In the next lesson, we introduce **unit testing** using **xUnit**.

The goal is to write tests for the Core project's game logic and understand how tests give you confidence when changing code.

To get the most out of the lesson, do the active tasks below before you arrive.

---

## 📖 Step 1 – Read the Core Project

Open `TicTacToe.Core` and read these files carefully:

- `Board.cs` — focus on `IsMoveValid`, `PlaceMove`, `CheckWin`, and `IsDraw`
- `GameEngine.cs` — focus on `SetPlayers`, `SetBoardSize`, and `TryPlayMove`
- `Player.cs` — notice the `Wins` property and `AddWin()`
- `GameStatus.cs` — three values: `InProgress`, `Win`, `Draw`

You do not need to understand every line. Focus on what each public method **receives** and what it **returns**.

---

## 🔍 Step 2 – Map the Testable Methods

For each method below, write down:
1. What inputs does it take?
2. What does it return or change?
3. What edge cases could go wrong?

| Method | What it does | Edge cases to think about |
|---|---|---|
| `Board.IsMoveValid(position)` | | |
| `Board.PlaceMove(position, symbol)` | | |
| `Board.CheckWin(symbol)` | | |
| `Board.IsDraw()` | | |
| `GameEngine.TryPlayMove(position)` | | |

Fill this in before the lesson. There are no wrong answers — this is to get you thinking.

---

## ✏️ Step 3 – Write One Test in Pseudo-Code

Before the lesson introduces xUnit syntax, try writing one test in plain English or pseudo-code.

Choose one behavior from the list below and write it out:

- "After placing X at position 1, that position is no longer valid"
- "A new board has all cells set to `.`"
- "After Alice wins, `Player1.Wins` equals 1"

Example format:

```
Given: a new Board of size 3
When:  PlaceMove(1, 'X') is called
Then:  IsMoveValid(1) returns false
```

Bring this to the lesson. You will translate it into real xUnit code.

---

## 🧠 Step 4 – Think About Position Mapping

The board uses **1-indexed positions**. Study how positions map to rows and columns:

```
1 | 2 | 3      (row 0)
4 | 5 | 6      (row 1)
7 | 8 | 9      (row 2)
```

Questions to think about:
- How many winning combinations exist on a 3×3 board?
- List them: which positions form each one?
- What sequence of moves ends in a draw (no winner, board full)?

---

## 🧠 Step 5 – What Unit Tests Are Not

Unit tests cover **pure logic**. They do not test:
- UI rendering
- Blazor components
- SignalR hubs or connections

Which parts of the project built in lessons 7 and 8 can be unit-tested? Which cannot?

Prepare a short answer to bring to the lesson.

---

## 🛠 Step 6 – Required Setup

Make sure you have before the lesson:

- The solution compiling with no errors (`dotnet build`)
- The `TicTacToe.Core` project with `Board.cs`, `GameEngine.cs`, `Player.cs`, `GameStatus.cs`

No test project is required yet — it will be created during the lesson.

---

## ✅ Ready for Lesson 9

You are ready if:
- You can describe what `TryPlayMove` takes and returns
- You have written at least one test in pseudo-code
- You know how many winning combinations exist on a 3×3 board

Next: **Writing our first xUnit tests for the Tic-Tac-Toe game** 🧪
