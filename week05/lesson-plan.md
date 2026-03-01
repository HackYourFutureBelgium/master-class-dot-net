# Lesson 5: Creating a Web API & Consuming It

## 📌 Lesson Overview
- Understanding RESTful APIs
- Setting up an ASP.NET Core Web API project
- Threads, async/await, and why they matter for web servers
- CancellationToken and cooperative cancellation
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

## 3️⃣ Threads, Async/Await, and Why They Matter

### The thread pool

A web server handles every incoming HTTP request on a **thread** — an independent unit of execution managed by the OS. .NET maintains a pool of ready-to-use threads so it does not pay the cost of creating a new one for every request.

The pool is finite. On a typical server it caps out at a few hundred threads. If every thread is busy, new requests queue up and wait.

```
Thread pool (simplified)

 Request → [Thread 1] → response
 Request → [Thread 2] → response
 Request → [Thread 3] → waiting for database...  ← thread blocked, doing nothing
 Request → [Thread 4] → waiting for database...  ← thread blocked, doing nothing
 Request →  no threads available → queued
```

Most web operations spend the majority of their time **waiting for I/O** — a database query, an HTTP call to another service, reading a file. A thread sitting idle during that wait is a thread that cannot serve another request.

**Async/await solves this.** When an async operation starts (e.g. a database query), the thread is returned to the pool. When the result is ready, a thread picks up where it left off. The thread never blocks.

```
 Request → [Thread 1] starts DB query → released back to pool
                  ↓ (DB working)
 Request → [Thread 1] handles a different request while waiting
                  ↓ (DB done)
 Request → [Thread 1] (or any free thread) resumes the first request
```

The key insight: **async/await is not about doing two things at once. It is about not wasting a thread while waiting for I/O.**

---

### Task\<T\>

`Task<T>` represents a **promise** of a future value. It is what an async method returns instead of the value directly.

```csharp
// Sync: blocks the thread until the DB responds
Player player = db.Players.Find(id);

// Async: releases the thread; resumes when DB responds
Player player = await db.Players.FindAsync(id);
```

`await` unwraps the `Task<T>` — it suspends the current method until the task completes, then continues with the result.

---

### The three rules

1. Mark the method `async`
2. Return `Task` (void) or `Task<T>` (value)
3. `await` every async call inside it

```csharp
// ❌ Synchronous — blocks a thread pool thread during the DB call
public IActionResult GetScore(int id)
{
    var score = _db.Scores.Find(id);   // thread blocks here
    return Ok(score);
}

// ✅ Asynchronous — thread is free during the DB call
public async Task<IActionResult> GetScore(int id)
{
    var score = await _db.Scores.FindAsync(id);   // thread released here
    return Ok(score);
}
```

> **Avoid `async void`.** Use `async Task` for methods that return nothing. `async void` swallows exceptions and cannot be awaited.

---

### Async in this lesson

`GameEngine` is pure in-memory logic — there is no I/O, so its methods stay synchronous. Controller actions are still written as `async Task<IActionResult>` because:

- It is the standard signature for ASP.NET Core actions
- In Lesson 6 the engine will call EF Core, and those calls are async — you will just add `await` without changing the structure
- The pattern is the same whether or not there is a real async call inside

---

## 4️⃣ CancellationToken

### What it is

A `CancellationToken` is a signal that a caller can send to say: **"stop what you are doing, the result is no longer needed."**

Common triggers in ASP.NET Core:
- The client closes the browser tab or navigates away
- The request times out
- The server is shutting down

Without cancellation, the server keeps working — querying the database, allocating memory, holding connections — for a result that nobody will ever read.

---

### How it works

A `CancellationTokenSource` owns the token and can cancel it. The token itself is passed around and checked.

```csharp
var cts = new CancellationTokenSource();
var token = cts.Token;

cts.Cancel(); // signals cancellation

token.IsCancellationRequested // → true
```

In practice you never create the source yourself in controllers. ASP.NET Core creates one per request and cancels it when the request is aborted. You just accept the token and pass it down.

---

### In controller actions

Add `CancellationToken cancellationToken` as the last parameter. ASP.NET Core binds it automatically from `HttpContext.RequestAborted`.

```csharp
[HttpGet("state")]
public async Task<IActionResult> GetGameState(CancellationToken cancellationToken)
{
    // pass the token to every async call that accepts one
    var state = await _db.GameStates.FirstOrDefaultAsync(cancellationToken);
    return Ok(state);
}
```

**The rule:** accept a `CancellationToken` in every public async method and pass it to every async call inside it. This lets cancellation propagate all the way down the call chain — from the HTTP layer to the database driver.

---

### What happens when it fires

When the token is cancelled, the next `await` that receives it throws `OperationCanceledException`. ASP.NET Core catches this and returns a `499 Client Closed Request` (or `503`) without logging it as an error.

You do not need to catch `OperationCanceledException` in controllers. Let it propagate.

```csharp
// ✅ correct — cancellation propagates naturally
public async Task<IActionResult> SlowAction(CancellationToken cancellationToken)
{
    var result = await _service.DoWorkAsync(cancellationToken);
    return Ok(result);
}
```

---

### When the game has no real async calls

For actions that call synchronous game logic, there is no async call to pass the token to. Still declare it — it signals intent and costs nothing, and the signature is already correct for when EF Core calls arrive in Lesson 6.

```csharp
[HttpPost("move")]
public async Task<IActionResult> MakeMove(
    [FromBody] int position,
    CancellationToken cancellationToken)
{
    // pure in-memory logic today — no async call to pass the token to
    // in Lesson 6: await _db.SaveChangesAsync(cancellationToken)
    if (!_engine.TryPlayMove(position))
        return BadRequest(new Warning("Invalid move"));

    return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
}
```

---

## 5️⃣ Creating the GameController

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
    public async Task<IActionResult> GetGameState(CancellationToken cancellationToken)
    {
        // pure in-memory today; in Lesson 6 this will await a DB call
        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(Status), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Warning), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Start(
        [FromBody] StartRequest request,
        CancellationToken cancellationToken)
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
    public async Task<IActionResult> MakeMove(
        [FromBody] int position,
        CancellationToken cancellationToken)
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
    public async Task<IActionResult> Undo(CancellationToken cancellationToken)
    {
        if (!_engine.TryUndoLastMove())
            return BadRequest(new Warning("No moves to undo"));

        return Ok(new Status(_engine.Status, _engine.CurrentPlayer));
    }
}
```

---

## 6️⃣ Creating the Request/Response model

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

## 7️⃣ Testing the API (Optional)

Use tools like:
- **Postman**
- **cURL**
- Or your browser for `GET` endpoints

Example request with cURL:

```bash
curl -X POST http://localhost:5000/api/game/status -H "Content-Type: application/json" -d "5"
```

---

## 8️⃣ Calling the API from MVC (Client Options)

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

## 9️⃣ Error Handling & Status Codes

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
