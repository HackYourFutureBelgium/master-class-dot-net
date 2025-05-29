# Lesson 9: Improving Performance & Game Logic

## 📌 Lesson Overview

This lesson is focused on polishing your app: optimizing performance, improving code quality, and using development tools effectively.

---

## 1️⃣ Profiling & Bottlenecks

Use Rider’s built-in tools to analyze performance:
- **Performance Profiler**: Identify expensive operations
- **Debugger**: Step into game logic and API calls
- **Logs**: Use structured logging to trace execution

```csharp
_logger.LogInformation("Player {PlayerId} made a move at {Index}", playerId, index);
```

---

## 2️⃣ Optimizing LINQ Queries

Avoid unnecessary allocations and loops:

```csharp
// Inefficient
var winners = games.Where(g => g.Winner != null).ToList();

// Better
var winners = games.Where(g => g.Winner != null).Select(g => g.WinnerId).ToList();
```

- Use `Any()` instead of `Count() > 0`
- Use `FirstOrDefault()` instead of `Where().FirstOrDefault()`

---

## 3️⃣ Refactoring Game Logic

Clean up your `GameService`:

- Separate concerns:
  - Game validation
  - Move registration
  - Win condition checking

Example:

```csharp
public bool IsMoveValid(int index) => index >= 0 && index < 9 && board[index] == null;
```

- Make methods small and readable
- Use enums and constants instead of magic strings

---

## 4️⃣ Caching Strategies (Optional)

If time allows:
- Use `IMemoryCache` for transient state
- Explain how caching can reduce pressure on APIs or databases

---

## 5️⃣ Debugging in Rider

Walk through debugging a game:

- Breakpoints in GameService
- Conditional breakpoints for specific scenarios
- Watch variables and evaluate expressions
- Step into LINQ queries and loops

---

## 6️⃣ Logging & Error Reporting

Consistent logging strategy:

```csharp
try
{
    MakeMove();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error making move");
    throw;
}
```

Add logging to API, GameService, and SignalR events.

---

## 7️⃣ Code Cleanup

- Remove unused code, services, and DI registrations
- Organize files and folders
- Improve naming conventions
- Revisit tests and ensure coverage (optional)

---

## 8️⃣ Wrap-up

By the end of this lesson, you can:
- Diagnose performance issues
- Write clearer, cleaner code
- Use LINQ and debugging tools effectively
- Prepare your solution for additional features

Next up: 🎉 Final Enhancements & Extensions!
