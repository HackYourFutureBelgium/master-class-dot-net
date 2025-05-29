# Lesson 4: Introducing MVC & Web Applications

## 📌 Lesson Overview
- Understanding the MVC architecture
- Setting up an ASP.NET Core MVC project
- Displaying game state using Razor Views
- Handling user moves through controllers and forms
- Routing and passing data between requests
- Validating input and handling errors

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
dotnet new mvc -n TicTacToe.Web
```

Add a reference to the shared game logic project:

```bash
dotnet add reference ../TicTacToe.Core/TicTacToe.Core.csproj
```

You should now have:
```
/TicTacToe.Solution
  /TicTacToe.Core
  /TicTacToe.Web
```

---

## 3️⃣ Building the GameController

In `Controllers/GameController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core;

public class GameController : Controller
{
    private static GameEngine _game = new GameEngine();

    public IActionResult Index()
    {
        return View(_game);
    }

    [HttpPost]
    public IActionResult MakeMove(int position)
    {
        if (_game.IsMoveValid(position))
        {
            _game.PlaceMove(position);

            if (_game.CheckWin() || _game.IsDraw())
                return RedirectToAction("End");

            _game.SwitchPlayer();
        }

        return RedirectToAction("Index");
    }

    public IActionResult End()
    {
        return View(_game);
    }
}
```

---

## 4️⃣ Creating the Views

### 🖼 Views/Game/Index.cshtml

```cshtml
@model TicTacToe.Core.GameEngine

<h2>Player @Model.CurrentPlayer, make your move</h2>

<form method="post" asp-action="MakeMove">
    <div>
        @for (int i = 0; i < 9; i += 3)
        {
            <div>
                @for (int j = 0; j < 3; j++)
                {
                    var pos = i + j + 1;
                    <button type="submit" name="position" value="@pos">
                        @(char.IsDigit(Model.Board[pos - 1]) ? pos.ToString() : Model.Board[pos - 1].ToString())
                    </button>
                }
            </div>
        }
    </div>
</form>
```

### 🎉 Views/Game/End.cshtml

```cshtml
@model TicTacToe.Core.GameEngine

<h2>Game Over</h2>

@if (Model.CheckWin())
{
    <p>🎉 Player @Model.CurrentPlayer wins!</p>
}
else
{
    <p>🤝 It's a draw!</p>
}

<a href="@Url.Action("Index", "Game")">Play again</a>
```

---

## 5️⃣ Routing & Navigation

- Default route: `/Game/Index`
- The form posts to `/Game/MakeMove`
- The result redirects back to `Index` or to `End`

You can inspect or customize routing in `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Game}/{action=Index}/{id?}");
```

---

## 6️⃣ Error Handling

### ✅ Server-side validation:

In `MakeMove()`:

```csharp
if (position < 1 || position > 9)
{
    TempData["Error"] = "Invalid move.";
    return RedirectToAction("Index");
}
```

### ✅ Display error in the view:

```cshtml
@if (TempData["Error"] != null)
{
    <p style="color:red">@TempData["Error"]</p>
}
```

---

## 🚀 End of Lesson 4
