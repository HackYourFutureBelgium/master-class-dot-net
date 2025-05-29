# Lesson 8: Multiplayer with SignalR

## 📌 Lesson Overview

In this lesson, we introduce multiplayer functionality using SignalR, allowing real-time communication between players.

---

## 1️⃣ What is SignalR?

- SignalR is a real-time communication library for ASP.NET Core.
- It allows servers to push updates to connected clients instantly.
- Ideal for chat apps, games, notifications, dashboards, etc.

---

## 🧱 Solution Structure & Communication

You now have three projects:

```
/TicTacToe.Solution
  /TicTacToe.Core      ← Game logic
  /TicTacToe.Api       ← Web API + SignalR Hub
  /TicTacToe.Blazor    ← Blazor UI connecting to SignalR
```

### 🔗 Communication Flow

- `TicTacToe.Blazor` connects to SignalR Hub hosted in `TicTacToe.Api`.
- Moves are sent from the Blazor app to the API using WebSockets.
- API broadcasts updates to all clients connected to the same game.

---

## 📦 Required NuGet Packages

### In `TicTacToe.Api` (SignalR Hub)

```bash
dotnet add package Microsoft.AspNetCore.SignalR
```

### In `TicTacToe.Blazor` (SignalR Client)

```bash
dotnet add package Microsoft.AspNetCore.SignalR.Client
dotnet add package Microsoft.AspNetCore.SignalR.Protocols.Json
```

---

## 2️⃣ Creating a SignalR Hub

```csharp
public class GameHub : Hub
{
    public async Task SendMove(int position, string symbol)
    {
        await Clients.Others.SendAsync("ReceiveMove", position, symbol);
    }
}
```

Register in `Program.cs` of `TicTacToe.Api`:

```csharp
builder.Services.AddSignalR();
app.MapHub<GameHub>("/gamehub");
```

If API and Blazor are on different origins, configure CORS to allow cross-origin SignalR connections.

---

## 3️⃣ Connecting from Blazor

```csharp
HubConnection hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:5001/gamehub")
    .Build();

await hubConnection.StartAsync();
```

---

## 4️⃣ Sending and Receiving Messages

### Send a move

```csharp
await hubConnection.SendAsync("SendMove", index, "X");
```

### Handle received move

```csharp
hubConnection.On<int, string>("ReceiveMove", (index, symbol) =>
{
    board[index] = symbol;
    StateHasChanged();
});
```

---

## 5️⃣ Handling Disconnects

```csharp
hubConnection.Closed += async (error) =>
{
    errorMessage = "Connection lost.";
    await Task.Delay(2000);
    await hubConnection.StartAsync();
};
```

---

## 6️⃣ Managing Players

- Assign each player a connection ID.
- Store game sessions in-memory or via API.
- Optionally pair users manually (enter game code) or automatically.

---

## 7️⃣ UI Integration Tips

- Disable input when it's not the player's turn.
- Show player names or roles (e.g., "Player X", "Player O").
- Alert the player when the opponent disconnects.

---

## 8️⃣ Wrap-up

By the end of this lesson you can:
- Create and register a SignalR hub
- Connect to it from Blazor
- Send and receive messages in real time
- Handle multiplayer scenarios with basic coordination

Next: Polishing the experience and improving performance! 🧼
