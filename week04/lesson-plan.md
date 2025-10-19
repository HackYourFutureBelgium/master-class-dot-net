# Lesson 4: Introducing MVC & Web Applications

## 📌 Lesson Overview
- Understanding the MVC architecture
- Setting up an ASP.NET Core MVC project
- Displaying game state using Razor Views
- Handling user moves through controllers and forms

---

## 1️⃣ What is MVC?

MVC stands for:
- **Model**: The data and business logic
- **View**: The UI
- **Controller**: The logic that handles requests, updates models, and returns views

This architecture separates responsibilities, making code easier to test, maintain, and expand.

---

## 2️⃣ Setting Up the Project

Use the .NET CLI or Rider to scaffold an ASP.NET Core MVC app:

```bash
dotnet new mvc -n TicTacToe.Api
```

Add a reference to the shared game logic project:

```bash
dotnet add reference ../TicTacToe.Core/TicTacToe.Core.csproj
```

You should now have:
```
/TicTacToe.Solution
  /TicTacToe.Api
  /TicTacToe.Core
```

---

## 3️⃣ Building the GameController

In `Controllers/GameController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core;

namespace TicTacToe.Web.Controllers;

public class GameController : Controller
{
    private readonly GameEngine _engine;
    
    public GameController(GameEngine engine)
    {
        _engine = engine;
    }

    [HttpGet]
    public IActionResult Play()
    {
        var player1 = new Player("Player 1", 'X');
        var player2 = new Player("Player 2", 'O');
        _engine.SetPlayers(player1, player2);
        _engine.SetBoardSize(3);
        return View(_engine);
    }

    [HttpPost]
    public IActionResult MakeMove(int position)
    {
        _engine.TryPlayMove(position);

        if (_engine.Status == GameStatus.Win || _engine.Status == GameStatus.Draw)
            return RedirectToAction("End");

        return View("Play", _engine);
    }

    [HttpGet]
    public IActionResult End()
    {
        return View(_engine);
    }
}
```

---

## 4️⃣ Creating the Views

### 🖼 Views/Game/Play.cshtml

```html
@using TicTacToe.Core

@model TicTacToe.Core.GameEngine

<h2>@Model.CurrentPlayer.Name (@Model.CurrentPlayer.Symbol)</h2>

<div class="row justify-content-center mt-4">
    <form method="post" asp-action="MakeMove" class="col-auto">
        <div class="d-grid gap-2">
            @for (var i = 0; i < Model.Board.Size; i++)
            {
                <div class="d-flex justify-content-center">
                    @for (var j = 0; j < Model.Board.Size; j++)
                    {
                        var pos = i * Model.Board.Size + j + 1;
                        var cell = Model.Board.GetCell(i, j);
                        <button type="submit" name="position" value="@pos"
                                class="btn btn-outline-dark m-1"
                                style="width: 60px; height: 60px; font-size: 24px"
                                @(cell != '.' || Model.Status != GameStatus.InProgress ? "disabled" : "")>
                            @(cell == '.' ? "" : cell.ToString())
                        </button>
                    }
                </div>
            }
        </div>
    </form>
</div>
```

### 🖼 Views/Game/End.cshtml

```html
@model TicTacToe.Core.GameEngine

<h2>Game Over</h2>

@switch (Model.Status)
{
    case TicTacToe.Core.GameStatus.Win:
        <p>🎉 Player @Model.CurrentPlayer.Name wins!</p>
        break;
    case TicTacToe.Core.GameStatus.Draw:
        <p>🤝 It's a draw!</p>
        break;
}

<a href="@Url.Action("Play", "Game")">Play again</a>
```

### 🖼 Views/Shared/_Layout.cshtml

Add a link to the game in the navigation bar:

```html
...
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Game" asp-action="Play">Game</a>
</li>
...
```

## 5️⃣ Registering the required services

In `Program.cs`:

```csharp
...
builder.Services.AddSingleton<GameStatsService>();
builder.Services.AddSingleton<GameEngine>();
...
```

---

## 🚀 End of Lesson 4
