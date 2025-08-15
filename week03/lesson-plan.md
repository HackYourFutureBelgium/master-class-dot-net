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

In .NET, the `List<T>` class is the most commonly used collection. It stores items in a resizable array-like structure and supports indexing, sorting, and filtering.

### 🧱 Creating a List

```csharp
var moves = new List<string> { "X", "O", "X" };
moves.Add("O");
Console.WriteLine(moves[2]); // Output: X
```

### 🔎 Iterating and Searching

```csharp
foreach (var move in moves)
    Console.WriteLine(move);

if (moves.Contains("X"))
    Console.WriteLine("X has played.");
```

---

## 2️⃣ LINQ – Language Integrated Query

LINQ provides query capabilities directly in C#. It’s useful to filter, transform, or group items from collections.

```csharp
var xMoves = moves.Where(m => m == "X").Count();
```

### 🧰 More Examples

```csharp
var recentMoves = moves.TakeLast(2);

var distinctMoves = moves.Distinct();

var grouped = moves.GroupBy(m => m)
                   .Select(g => $"{g.Key} played {g.Count()} times");
```

<details>

<summary>Concrete example</summary>

### The `Board.IsDraw()` Method

Originally written as:

```csharp
public bool IsDraw()
{
   foreach (var cell in _cells)
      if (cell == '.') return false;
   
   return true;
}
```

Can be simplified using LINQ:

```csharp
public bool IsDraw()
    => _cells.Cast<char>().All(cell => cell != '.');
```
</details>

---

## 3️⃣ Moving Game Logic to a Class Library

Our game logic is getting more complex. To keep things modular and reusable, we'll extract it into a **class library project**.

### 📦 Step-by-Step

1. Create a new class library project:

   ```bash
   dotnet new classlib -n TicTacToe.Core
   ```

2. Move all relevant game logic classes (e.g., `Board`, `Player`, `GameController`, etc.) into this project.

3. Make sure all moved classes use a proper namespace, e.g.:

   ```csharp
   namespace TicTacToe.Core;
   ```

4. Reference the core logic from your console app:

   ```bash
   dotnet add TicTacToe.Console reference ../TicTacToe.Core/TicTacToe.Core.csproj
   ```

This separation makes it easier to test, reuse, and plug the game logic into different interfaces like MVC, Web API, or Blazor in later lessons.

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