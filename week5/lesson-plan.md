# Lesson 5: Creating a Web API & Consuming It

## 📌 Lesson Overview
- Understanding RESTful APIs
- Setting up an ASP.NET Core Web API project
- Exposing game logic via API endpoints
- Consuming the API from a Razor View using JavaScript or HttpClient
- Error handling with proper HTTP status codes and JSON responses

---

## 1️⃣ What Is a Web API?

- A **Web API** is a set of HTTP-accessible endpoints that send and receive data (usually JSON).
- It is **controller-only**, without rendering views.
- It follows **REST principles** (representational state transfer):
  - Use nouns for resources: `/api/game`
  - Use HTTP verbs to indicate action: `GET`, `POST`, `PUT`, `DELETE`

---

## 2️⃣ Setting Up the Project

Create a Web API project:

```bash
dotnet new webapi -n TicTacToe.Api
```

Reference the shared game logic:

```bash
dotnet add reference ../TicTacToe.Core/TicTacToe.Core.csproj
```

Disable Swagger if not needed for now:

```csharp
builder.Services.AddControllers(); // already included
// comment out builder.Services.AddEndpointsApiExplorer();
// comment out builder.Services.AddSwaggerGen();
```

---

## 3️⃣ Creating the GameApiController

In `Controllers/GameApiController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core;

[ApiController]
[Route("api/[controller]")]
public class GameApiController : ControllerBase
{
    private static GameEngine _game = new GameEngine();

    [HttpGet("state")]
    public IActionResult GetGameState()
    {
        return Ok(new
        {
            Board = _game.Board,
            CurrentPlayer = _game.CurrentPlayer
        });
    }

    [HttpPost("move")]
    public IActionResult MakeMove([FromBody] int position)
    {
        if (!_game.IsMoveValid(position))
            return BadRequest(new { Message = "Invalid move." });

        _game.PlaceMove(position);

        if (_game.CheckWin())
            return Ok(new { Message = "Win", Winner = _game.CurrentPlayer });

        if (_game.IsDraw())
            return Ok(new { Message = "Draw" });

        _game.SwitchPlayer();
        return Ok(new { Message = "Continue", NextPlayer = _game.CurrentPlayer });
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _game = new GameEngine();
        return Ok(new { Message = "Game reset" });
    }
}
```

---

## 4️⃣ Testing the API (Optional)

Use tools like:
- **Postman**
- **cURL**
- Swagger UI (enabled by default)
- Or your browser for `GET` endpoints

Example request with cURL:

```bash
curl -X POST http://localhost:5000/api/gameapi/move -H "Content-Type: application/json" -d "5"
```

---

## 5️⃣ Calling the API from MVC (Client Options)

You can consume the API from:
- ✅ JavaScript (fetch)
- ✅ MVC controller (`HttpClient`)
- ✅ Blazor (in a future lesson)

### Example using JavaScript fetch in a Razor View:

```html
<script>
async function makeMove(pos) {
    const response = await fetch('/api/gameapi/move', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(pos)
    });

    const result = await response.json();
    alert(result.message);
}
</script>

<button onclick="makeMove(1)">Move 1</button>
```

---

## 6️⃣ Error Handling & Status Codes

- Use `400 Bad Request` for invalid input
- Use `200 OK` with a clear message on success
- Use `500` if internal logic fails (with optional `try-catch`)

### Example response:

```json
{
  "message": "Win",
  "winner": "X"
}
```

---

## 🚀 End of Lesson 5
