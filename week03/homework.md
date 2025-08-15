# 🏠 Homework – Lesson 3: Collections, LINQ & Structuring Code

## 🎯 Objective

Extend your game logic using collections and LINQ, and practice structuring your code into reusable services. This homework builds on what you’ve done in class and pushes it further with additional logic and data tracking.

---

## 📌 Tasks

### 1. Track the history of all moves

- Create a `List<Move>` to store all moves played in the current game.
- Define a `Move` class:

```csharp
public class Move
{
    public int Position { get; set; }
    public char Symbol { get; set; }
    public DateTime Timestamp { get; set; }
}
```

- Add a new `Move` to the list after every valid turn.

---

### 2. List previous moves using LINQ

- Add a method that lists all moves chronologically.
- Use LINQ to sort the list by `Timestamp`.
- Output example:

```
Player X played at 5 on 2024-04-08 14:32:01
Player O played at 1 on 2024-04-08 14:32:15
```

---

### 3. Game statistics with LINQ

- Count how many times each position has been played across all rounds.
- Use `GroupBy` and `Count()` to print something like:

```
Position 5 was played 3 times
Position 1 was played 2 times
```

---

### 4. Refactor into a `GameStatsService`

- Create a class to handle all move tracking logic:

```csharp
public class GameStatsService
{
    public void AddMove(Move move) { ... }
    public void ClearHistory() { ... }
}
```

- Use this service from your main game loop.

---

## 🧩 Bonus Challenges

- Implement "undo last move" functionality using your move history.
- Export all moves to a `.txt` file after each game.
- Add an option to replay a finished game from its move list.

---

Good luck and have fun! 🎮
