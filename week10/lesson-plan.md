# Lesson 10: Final Enhancements & Closing

## 📌 Lesson Overview

In this final lesson, we extend the Tic Tac Toe game with optional features, review everything we've learned, and share resources for continuing your .NET journey.

---

## 1️⃣ Feature Extensions

### 🏆 Scoring System

Add a service to track win/loss/draw records per player.

```csharp
public class ScoreTracker
{
    private readonly Dictionary<string, int> _scores = new();

    public void RecordWin(string playerName)
    {
        if (!_scores.ContainsKey(playerName))
            _scores[playerName] = 0;

        _scores[playerName]++;
    }

    public int GetScore(string playerName) =>
        _scores.TryGetValue(playerName, out var score) ? score : 0;
}
```

### 🔁 Rematch Support

- Add a “Play Again” button to the UI
- Reset board and state without reloading the page

---

## 2️⃣ Adding a Simple AI Opponent

Let students explore basic AI logic:

```csharp
public int GetRandomMove(string?[] board)
{
    var available = board
        .Select((value, index) => new { value, index })
        .Where(cell => cell.value == null)
        .Select(cell => cell.index)
        .ToList();

    return available[new Random().Next(available.Count)];
}
```

Advanced (optional): Introduce Minimax concept for unbeatable AI.

---

## 3️⃣ Polishing the UX

- Add hover effects and active player highlights
- Improve layout, spacing, mobile responsiveness
- Use toast messages for win/loss notifications

---

## 4️⃣ Recap of Key Concepts

Review each phase of the course:
- ✅ C# basics: types, loops, arrays, exceptions
- ✅ OOP: classes, properties, encapsulation
- ✅ Design patterns: Singleton, layering, DI
- ✅ Projects: Console → MVC → API → Blazor → SignalR
- ✅ Tools: LINQ, EF Core, ADO.NET, Debugging, Logging

---

## 5️⃣ Resources for Going Further

- [Microsoft Learn – C# Path](https://learn.microsoft.com/en-us/training/paths/csharp-first-steps/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- Blazor tutorials, SignalR samples, GitHub open source projects

---

## 🏁 Wrap-up

- Congratulate students for completing a full-stack project
- Encourage them to keep building, learning, and exploring
- Gather feedback and share your GitHub repo for further practice

Thank you and well done! 🎉
