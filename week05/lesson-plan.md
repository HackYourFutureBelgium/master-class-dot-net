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

Add controller services in `Program.cs` (if missing):

```csharp
builder.Services.AddControllers();
```

---

## 3️⃣ Creating the GameController

In `Controllers/GameController.cs`:

```csharp
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Core;
using TicTacToe.Api.Requests;
using TicTacToe.Api.Responses;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class GameApiController : ControllerBase
{
    private readonly GameEngine _engine;

    public GameApiController(GameEngine engine)
    {
        _engine = engine;
    }

    [HttpGet("state")]
    public IActionResult GetGameState() => Ok(new Status(_engine.Status, _engine.CurrentPlayer));

    [HttpPost("start")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Start([FromBody] StartRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Player1Name)
         || string.IsNullOrWhiteSpace(request.Player2Name)
         || request.BoardSize < 3
         || request.BoardSize > 9)
        {
            return BadRequest(new Warning("Invalid input data"));
        }

        if (request.Player1Name == request.Player2Name)
        {
            return BadRequest(new Warning("Players cannot have the same name"));
        }

        var player1 = new Player(request.Player1Name, 'X');
        var player2 = new Player(request.Player2Name, 'O');
        _engine.SetPlayers(player1, player2);
        _engine.SetBoardSize(request.BoardSize);
        return CreatedAtAction(nameof(GetGameState), new Status(_engine.Status, _engine.CurrentPlayer));
    }

    [HttpPost("move")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult MakeMove([FromBody] int position)
    {
        if (_engine.Status != GameStatus.InProgress)
            return BadRequest();

        if (!_engine.TryPlayMove(position))
            return BadRequest(new Warning("Invalid move"));

        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }

    [HttpPost("undo")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Undo()
    {
        if (!_engine.TryUndoLastMove())
            return BadRequest(new Warning("No moves to undo"));

        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }
}
```

---

## 4️⃣ Creating the Request/Response model

In `Requests/StartRequest.cs`:

```csharp
namespace TicTacToe.Api.Requests;

public record StartRequest(string Player1Name, string Player2Name, int BoardSize);

```

In `Responses/Status.cs`:

```csharp
namespace TicTacToe.Api.Requests;

public record Status(GameStatus GameStatus, Player CurrentPlayer);

```

In `Responses/Warning.cs`:

```csharp
namespace TicTacToe.Api.Requests;

public record Warning(string Message);

```

---

## 5️⃣ Testing the API (Optional)

Use tools like:
- **Postman**
- **cURL**
- Or your browser for `GET` endpoints

Example request with cURL:

```bash
curl -X POST http://localhost:5000/api/game/status -H "Content-Type: application/json" -d "5"
```

---

## 6️⃣ Calling the API from MVC (Client Options)

You can consume the API from:
- ✅ JavaScript (fetch)
- ✅ MVC controller (`HttpClient`)
- ✅ Blazor (in a future lesson)

### Example using JavaScript fetch in a Razor View:

```html
<script>
async function makeMove(pos) {
    const response = await fetch('/api/game/move', {
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

## 7️⃣ Error Handling & Status Codes

- Use `400 Bad Request` for invalid input
- Use `200 OK` with a clear message on success
- Use `500` if internal logic fails (with optional `try-catch`)

### Example response:

```json
{
  "gameStatus": 1,
  "currentPlayer": {
    "name": "Player 1",
    "symbol": "X",
    "wins": 2
  }
}
```

---

## 🚀 End of Lesson 5
