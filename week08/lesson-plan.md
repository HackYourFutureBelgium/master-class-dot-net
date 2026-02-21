# Lesson 8: Real-Time Communication with SignalR

## 📌 Lesson Overview

In this lesson, we introduce **SignalR**, the real-time communication framework used by ASP.NET Core.

You will learn:
- What SignalR is and when to use it
- How SignalR works in **Blazor Server**
- How to define and register a SignalR Hub
- How to broadcast messages from a **Blazor component** using `IHubContext<T>`
- How components receive and react to real-time messages
- How SignalR enables multiplayer and collaborative scenarios

---

## 🧭 Lesson Flow (Recommended)

1. What SignalR is and why it exists
2. How SignalR fits into Blazor Server
3. Hub basics — the hub as a typed channel with a `JoinGame` method
4. Registering the hub
5. From one game to many — `GameRoomManager`, URL-based game IDs
6. Broadcasting from a component via `IHubContext<T>` to a SignalR group
7. Connecting, joining a group, and subscribing to messages
8. Pitfalls + common mistakes
9. Handling disconnects
10. Connecting from JavaScript — live dashboard demo

---

## 🧠 Mental Model (Beginner View)

- A **Hub** is a typed channel — clients connect to it and subscribe to messages.
- A **Blazor component** still drives the game logic, just like in lesson 7.
- **`IHubContext<T>`** lets a component push a notification to all connected clients.
- Every component listens for that notification via a `HubConnection` and re-renders.

```
User A's component → Engine.TryPlayMove() → IHubContext → all connected clients
                                                               ↓
                                                    HubConnection.On → StateHasChanged
```

---

## 1️⃣ What is SignalR?

**SignalR** is a library for adding **real-time web functionality** to applications.

Real-time means:
- The server can push data to connected clients immediately
- Clients do not need to poll or refresh
- All connected users can stay in sync

Typical use cases:
- Multiplayer games
- Chat applications
- Live dashboards
- Collaborative tools (shared editors, boards, etc.)
- Notifications and presence tracking

SignalR works on top of:
- WebSockets (preferred)
- Server-Sent Events (fallback)
- Long polling (fallback)

The transport is chosen automatically.

---

## 2️⃣ SignalR in Blazor Server (Important Clarification)

In **Blazor Server**, SignalR is **already in use**.

Blazor Server works like this:
- UI runs on the server
- The browser maintains a persistent SignalR connection
- UI events (clicks, inputs) are sent to the server
- UI updates are pushed back to the browser

So:
- You are *already using SignalR*
- You are now learning how to **use it explicitly**

👉 This lesson focuses on **custom SignalR hubs**, not the internal Blazor one.

---

## 🧱 Solution Structure

Your solution contains **two projects**:

```
/TicTacToe.Solution
  /TicTacToe.Core   ← Game rules & domain logic
  /TicTacToe.Web    ← Blazor Server + SignalR
```

Key principles:
- `Core` contains no SignalR code
- `Web` hosts the hub and the Blazor components
- Components drive game logic via `GameEngine`, just like in lesson 7
- SignalR notifies all connected components when state changes
- The server is authoritative (clients do not decide game state)

---

## 3️⃣ How SignalR Works (Conceptually)

SignalR introduces the concept of a **Hub**.

A hub:
- Lives on the server
- Acts as a typed channel clients connect to
- Can push messages to one client, many clients, or all clients

In this architecture, the hub itself is **thin** — it does not contain game logic. The Blazor component does the work, just as it did in lesson 7, and uses the hub to notify other connected clients.

Communication is **one-directional for push**:

```
Component → IHubContext<GameHub> → All connected clients
```

Components call `GameEngine` directly (same as before) and broadcast a notification via SignalR so other clients know to re-render.

---

## 4️⃣ Creating a SignalR Hub

A hub is a C# class that inherits from `Hub`. In this lesson the hub has a single method: `JoinGame`, which adds the caller's connection to a SignalR **group** named after the game ID.

```csharp
using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }
}
```

For the game room, this is the only hub method needed. All game logic stays in the Blazor component, and all broadcasts are sent from the component via `IHubContext<GameHub>`. (Section 🔟 extends the hub with a `JoinLobby` method for the dashboard.)

---

## 5️⃣ Registering the Hub

In `Program.cs` of the **Blazor Server project**:

```csharp
builder.Services.AddSignalR();
```

Map the hub:

```csharp
app.MapHub<GameHub>("/gamehub");
```

This exposes the hub at:

```
/gamehub
```

No API project, no CORS, no cross-origin configuration is required.

---

## 5️⃣a From One Game to Many

In lesson 7, `GameEngine` was a singleton — one shared instance for the entire server. That worked for a local single-player demo, but breaks the moment two different browser tabs try to play different games: they would both be looking at the same board.

For true multiplayer we need **one `GameEngine` per game room**, created and looked up by a game ID.

The problem with just injecting `GameEngine` directly:

```csharp
// ❌ This always gives every component the same engine
@inject GameEngine Engine
```

The solution is a manager that creates and stores one engine per game ID:

```csharp
// ✅ Each game ID gets its own engine
GameEngine engine = RoomManager.GetOrCreate(gameId);
```

---

## 5️⃣b GameRoomManager

`GameRoomManager` is a singleton service that owns a dictionary of game rooms:

```csharp
public class GameRoomManager
{
    private readonly ConcurrentDictionary<string, GameEngine> _rooms = new();
    private readonly ILoggerFactory _loggerFactory;

    public GameRoomManager(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public GameEngine GetOrCreate(string gameId)
        => _rooms.GetOrAdd(gameId, _ =>
            new GameEngine(_loggerFactory.CreateLogger<GameEngine>(), new GameStatsService()));
}
```

Key points:
- `ConcurrentDictionary` makes it safe for multiple components accessing it at the same time
- `GetOrAdd` creates the engine only once per game ID, then returns the same instance
- Each engine gets its own `GameStatsService` (its own move history)
- Register it in `Program.cs` as a singleton: `builder.Services.AddSingleton<GameRoomManager>()`

---

## 5️⃣c URL-Based Game IDs and Route Parameters

Each game room needs a unique ID. The cleanest approach is to put it in the URL so players can share a link.

A Blazor component receives a URL segment as a route parameter:

```razor
@page "/game/{GameId}"

@code {
    [Parameter] public string? GameId { get; set; }
}
```

Navigating to `/game/room42` sets `GameId = "room42"`.

The home page lets players type a game ID and navigate:

```razor
@inject NavigationManager Nav

<input @bind="gameId" placeholder="e.g. room42" />
<button @onclick="@(() => Nav.NavigateTo($"/game/{gameId}"))">Join Game</button>
```

Two players who navigate to the same URL will share the same `GameEngine` and the same SignalR group.

The lobby (game ID entry form) lives inside the same `Game.razor` component — when `GameId` is empty (route `/game`), the component renders the form; when `GameId` is set (route `/game/room42`), it renders the board.

---

## 6️⃣ Broadcasting from a Blazor Component with IHubContext

This is the **central pattern** for this lesson.

`IHubContext<T>` is a service you inject into a component to push notifications to all connected clients. It builds directly on what you already have from lesson 7.

The component handles **two routes** — the lobby (no game ID) and the game room (with game ID). When a player navigates to `/game/room42`, Blazor reuses the component instance and calls `OnParametersSetAsync`. The `_connectedGameId` guard ensures the connection is set up exactly once per game room.

```razor
@page "/game"
@page "/game/{GameId}"

@inject GameRoomManager RoomManager
@inject IHubContext<GameHub> HubContext
@inject NavigationManager Nav
@implements IAsyncDisposable

@code {
    [Parameter] public string? GameId { get; set; }

    private HubConnection? _connection;
    private string? _connectedGameId;

    private GameEngine? Engine => string.IsNullOrEmpty(GameId) ? null : RoomManager.GetOrCreate(GameId);

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(GameId) || GameId == _connectedGameId) return;

        _connectedGameId = GameId;

        if (_connection is not null)
            await _connection.DisposeAsync();

        _connection = new HubConnectionBuilder()
            .WithUrl(Nav.ToAbsoluteUri("/gamehub"))
            .Build();

        // Listen for notifications from other clients
        _connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));

        await _connection.StartAsync();

        // Join this game's SignalR group
        await _connection.SendAsync("JoinGame", GameId);
    }

    private async Task MakeMove(int position)
    {
        Engine.TryPlayMove(position);

        // Notify only the clients in this game's room
        await HubContext.Clients.Group(GameId).SendAsync("StateUpdated");
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
```

What happens here:
1. User A's component calls `Engine.TryPlayMove()` — same as lesson 7
2. Component sends a `"StateUpdated"` notification to all clients in the group via `HubContext`
3. Every connected component (including User B's) receives the notification
4. Each component calls `StateHasChanged()` and re-renders using the shared `GameEngine`

Because `GameRoomManager` is a singleton, every component in the same game room already has access to the same `GameEngine` — SignalR just tells them to re-render.

---

## 6️⃣a IHubContext Target Options

You can target different sets of clients:

```csharp
// All connected clients
await HubContext.Clients.All.SendAsync("StateUpdated");

// One specific client (by connection ID)
await HubContext.Clients.Client(connectionId).SendAsync("StateUpdated");

// All except one
await HubContext.Clients.AllExcept(connectionId).SendAsync("StateUpdated");

// A group (e.g., a game room)
await HubContext.Clients.Group(gameId).SendAsync("StateUpdated");
```

---

## 6️⃣b IHubContext Beyond Components

`IHubContext<T>` is not limited to Blazor components — it can be injected anywhere in the DI container:

- **Services** — broadcast when a background process completes
- **Background jobs** — push live dashboard updates
- **Controllers** — if your app also has an API layer, a controller can broadcast after handling an HTTP request

This is worth knowing for future projects, but in this course the component pattern is what you will use.

---

## 7️⃣ Connecting to the Hub from Blazor

Clients connect to the hub to subscribe to messages.

```csharp
HubConnection connection = new HubConnectionBuilder()
    .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
    .Build();

await connection.StartAsync();
```

This establishes a persistent connection for receiving server-pushed messages.

---

## 7️⃣a Connection Lifecycle (Where to Put It)

- Create and start the connection in `OnParametersSetAsync` (see section 7b for why, not `OnInitializedAsync`).
- Register handlers **before** starting.
- Call `JoinGame` **after** starting to enter this game's SignalR group.
- Dispose in `DisposeAsync`.

```csharp
private HubConnection? _connection;
private string? _connectedGameId;

protected override async Task OnParametersSetAsync()
{
    if (string.IsNullOrEmpty(GameId) || GameId == _connectedGameId) return;

    _connectedGameId = GameId;

    _connection = new HubConnectionBuilder()
        .WithUrl(Nav.ToAbsoluteUri("/gamehub"))
        .Build();

    // Register handler before starting
    _connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));

    await _connection.StartAsync();

    // Join this game's group so we receive its broadcasts
    await _connection.SendAsync("JoinGame", GameId);
}

public async ValueTask DisposeAsync()
{
    if (_connection is not null)
        await _connection.DisposeAsync();
}
```

---

## 7️⃣b OnParametersSetAsync vs OnInitializedAsync

When a component handles **two routes** (`@page "/game"` and `@page "/game/{GameId}"`), Blazor reuses the same component instance when navigating between them. This means `OnInitializedAsync` only runs once — on the first load at `/game` — and is **not called again** when the user navigates to `/game/room42`.

If you set up the hub connection in `OnInitializedAsync` with an early-return guard for empty `GameId`, the connection will never be created.

The fix is to use `OnParametersSetAsync` instead, with a guard to run only when `GameId` first becomes non-empty:

```csharp
private string? _connectedGameId;

protected override async Task OnParametersSetAsync()
{
    if (string.IsNullOrEmpty(GameId) || GameId == _connectedGameId) return;

    _connectedGameId = GameId;

    _connection = new HubConnectionBuilder()
        .WithUrl(Nav.ToAbsoluteUri("/gamehub"))
        .Build();

    _connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));

    await _connection.StartAsync();
    await _connection.SendAsync("JoinGame", GameId);
}
```

The `_connectedGameId` field prevents re-connecting every time any parameter changes.

---

## 8️⃣ Receiving Messages

Clients subscribe to messages using `On`:

```csharp
// Simple notification with no payload — just trigger a re-render
connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));

// With a typed payload (when the server sends data along with the message)
connection.On<GameState>("ReceiveGameState", state =>
{
    // Update local UI state
    return InvokeAsync(StateHasChanged);
});
```

Key idea:
- Method names are string-based and must exactly match what the server sends
- The no-payload form (`On("name", () => ...)`) is used when the broadcast is just a signal to re-render
- The typed form (`On<T>("name", data => ...)`) is used when the server sends data directly
- UI updates must trigger `StateHasChanged()`

---

## 8️⃣a UI Updates and StateHasChanged

- SignalR handlers run **outside** normal UI events (on a background thread).
- Always call `InvokeAsync(StateHasChanged)` to trigger a re-render from a SignalR handler — do not call `StateHasChanged()` directly.

Example (notification-only, no payload):

```csharp
connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));
```

Example (with payload, when the server sends data):

```csharp
connection.On<GameState>("ReceiveGameState", state =>
{
    gameState = state;
    return InvokeAsync(StateHasChanged);
});
```

---

## 8️⃣b Common Pitfalls (with Examples)

- **Handler name mismatch** (string must match exactly):

```csharp
// Server sends "StateUpdated"
await HubContext.Clients.Group(GameId).SendAsync("StateUpdated");

// Client listens to "stateupdated" (wrong — case-sensitive)
connection.On("stateupdated", () => InvokeAsync(StateHasChanged));
```

- **Updating UI without re-render**:

```csharp
connection.On("StateUpdated", () =>
{
    // Missing InvokeAsync(StateHasChanged)
});
```

- **Subscribing multiple times** (duplicate updates):

```csharp
// Calling this on every parameter change creates duplicate handlers
connection.On("StateUpdated", () => InvokeAsync(StateHasChanged));
```

Use the `_connectedGameId` guard in `OnParametersSetAsync` to ensure you subscribe exactly once.

- **Not starting the connection**:

```csharp
// Missing StartAsync, no messages will be received
await connection.StartAsync();
```

---

## 9️⃣ Handling Disconnects

Connections can drop at any time.

SignalR provides lifecycle hooks:

Client side:

```csharp
connection.Closed += async error =>
{
    // Update UI
    await Task.Delay(2000);
    await connection.StartAsync();
};
```

Server side (override in hub):

```csharp
public override Task OnDisconnectedAsync(Exception? exception)
{
    // Cleanup player state
    return base.OnDisconnectedAsync(exception);
}
```

---

## 🔟 Connecting from JavaScript

SignalR is not limited to Blazor or .NET clients. The official SignalR JavaScript client lets any web page connect to the **same hub** — no extra server code required.

This is useful for:
- Live spectator dashboards as plain HTML pages
- Non-Blazor frontends
- External monitoring tools

### The JavaScript API

Load the client from CDN:

```html
<script src="https://unpkg.com/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
```

The JS API mirrors the C# `HubConnection` API:

| `C#` | JavaScript |
|---|---|
| `new HubConnectionBuilder()` | `new signalR.HubConnectionBuilder()` |
| `.WithUrl("/gamehub")` | `.withUrl("/gamehub")` |
| `connection.On("name", handler)` | `connection.on("name", handler)` |
| `await connection.StartAsync()` | `await connection.start()` |
| `connection.SendAsync("method")` | `connection.invoke("method")` |

Key differences:
- Method names are **camelCase** in JavaScript (`.withUrl`, `.build`, `.start`, `.invoke`)
- Properties in received objects are also **camelCase** (e.g., `game.gameId`, not `game.GameId`) — ASP.NET Core serialises to camelCase by default

### Adding Lobby Support to the Hub

A `JoinLobby` method adds the caller to a `"lobby"` group and sends the current state immediately:

```csharp
public class GameHub : Hub
{
    private readonly GameRoomManager _roomManager;

    public GameHub(GameRoomManager roomManager)
    {
        _roomManager = roomManager;
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task JoinLobby()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "lobby");
        // Send the current snapshot immediately so the page loads with data
        await Clients.Caller.SendAsync("LobbyUpdated", _roomManager.GetActiveSummaries());
    }
}
```

### The GameSummary Record

When the server sends data to JavaScript, it serialises the object to JSON. A `GameSummary` record defines the shape:

```csharp
public record GameSummary(
    string GameId,
    string Player1Name,
    int Player1Wins,
    string Player2Name,
    int Player2Wins,
    string Status,
    string CurrentPlayer
);
```

`GameRoomManager` builds summaries from live engine state:

```csharp
public IEnumerable<GameSummary> GetActiveSummaries()
    => _rooms
        .Where(kv => kv.Value.Board != null)
        .Select(kv =>
        {
            var e = kv.Value;
            return new GameSummary(
                kv.Key,
                e.Player1?.Name ?? "?",
                e.Player1?.Wins ?? 0,
                e.Player2?.Name ?? "?",
                e.Player2?.Wins ?? 0,
                e.Status.ToString(),
                e.CurrentPlayer?.Name ?? "?"
            );
        });
```

### Broadcasting to the Lobby

Every game action now sends two broadcasts — one to the game room, one to the lobby:

```csharp
private async Task BroadcastUpdate()
{
    await HubContext.Clients.Group(GameId!).SendAsync("StateUpdated");
    await HubContext.Clients.Group("lobby").SendAsync("LobbyUpdated", RoomManager.GetActiveSummaries());
}
```

### The Dashboard Page

`wwwroot/dashboard.html` is a plain HTML file served as a static asset. It connects to the hub and renders a live table of all active games:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <title>Game Dashboard</title>
    <link rel="stylesheet" href="lib/bootstrap/dist/css/bootstrap.min.css" />
</head>
<body class="container mt-5">

    <h1>🎮 Live Game Dashboard</h1>
    <div id="status" class="alert alert-info">Connecting...</div>

    <table class="table table-bordered mt-3" id="games-table" style="display:none">
        <thead class="table-dark">
            <tr>
                <th>Game ID</th>
                <th>Player 1 (X)</th>
                <th>Player 2 (O)</th>
                <th>Status</th>
                <th>Current Turn</th>
            </tr>
        </thead>
        <tbody id="games-body"></tbody>
    </table>

    <p id="no-games" style="display:none">No active games right now.</p>

    <script src="https://unpkg.com/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
    <script>
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/gamehub")
            .build();

        // Subscribe before starting — same rule as in C#
        connection.on("LobbyUpdated", renderGames);

        connection.start()
            .then(() => {
                document.getElementById("status").className = "alert alert-success";
                document.getElementById("status").textContent = "Connected — watching live updates.";
                // Join the lobby group and receive the current snapshot immediately
                return connection.invoke("JoinLobby");
            })
            .catch(err => {
                document.getElementById("status").className = "alert alert-danger";
                document.getElementById("status").textContent = "Connection failed: " + err;
            });

        function renderGames(games) {
            const tbody   = document.getElementById("games-body");
            const table   = document.getElementById("games-table");
            const noGames = document.getElementById("no-games");

            tbody.innerHTML = "";

            if (games.length === 0) {
                table.style.display   = "none";
                noGames.style.display = "";
                return;
            }

            table.style.display   = "";
            noGames.style.display = "none";

            for (const game of games) {
                // Properties are camelCase: game.gameId, game.player1Name, etc.
                const row = document.createElement("tr");
                row.innerHTML =
                    `<td><strong>${game.gameId}</strong></td>` +
                    `<td>${game.player1Name} — ${game.player1Wins} win(s)</td>` +
                    `<td>${game.player2Name} — ${game.player2Wins} win(s)</td>` +
                    `<td>${formatStatus(game.status)}</td>` +
                    `<td>${game.status === "InProgress" ? game.currentPlayer : "—"}</td>`;
                tbody.appendChild(row);
            }
        }

        function formatStatus(status) {
            if (status === "InProgress") return '<span class="badge bg-success">In Progress</span>';
            if (status === "Win")        return '<span class="badge bg-primary">Win</span>';
            if (status === "Draw")       return '<span class="badge bg-secondary">Draw</span>';
            return status;
        }
    </script>

</body>
</html>
```

Navigate to `/dashboard.html` to open the dashboard — no Blazor, no server-side rendering, just SignalR and the browser.

---

## ✅ Wrap-Up

By the end of this lesson, you understand:

- What SignalR is and why it exists
- How SignalR fits into Blazor Server
- How to define and register a hub
- How to inject `IHubContext<T>` into a Blazor component to broadcast notifications
- How components connect, subscribe, and re-render when state changes
- How SignalR enables multiplayer **by building directly on what you built in lesson 7**

This knowledge applies to:
- Games
- Chats
- Dashboards
- Any real-time feature

**You now have a working multiplayer Tic-Tac-Toe game.** Multiple players can join different game rooms by sharing a URL, and every move is broadcast in real time to all connected clients.