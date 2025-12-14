# Lesson 8: Real-Time Communication with SignalR

## 📌 Lesson Overview

In this lesson, we introduce **SignalR**, the real-time communication framework used by ASP.NET Core.

You will learn:
- What SignalR is and when to use it
- How SignalR works in **Blazor Server**
- How to define and use a SignalR Hub
- How clients exchange real-time messages
- How SignalR enables multiplayer and collaborative scenarios

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
- `Web` hosts the SignalR hub
- The server is authoritative (clients do not decide game state)

---

## 3️⃣ How SignalR Works (Conceptually)

SignalR introduces the concept of a **Hub**.

A hub:
- Lives on the server
- Exposes methods clients can call
- Can send messages to one client, many clients, or all clients

Communication is **bidirectional**:

```
Client → Hub → Other Clients
Client ← Hub ← Other Clients
```

SignalR manages:
- Connections
- Connection IDs
- Groups (rooms, games, channels)
- Broadcasting

---

## 4️⃣ Creating a SignalR Hub

A hub is just a C# class that inherits from `Hub`.

```csharp
using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task SendMove(int position, char symbol)
    {
        await Clients.Others.SendAsync("ReceiveMove", position, symbol);
    }
}
```

Important points:
- Methods are `public`
- Methods return `Task`
- Parameters must be serializable
- `Clients.Others` sends to everyone except the caller

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

## 6️⃣ Connecting to the Hub from Blazor

In Blazor Server, you create a `HubConnection` manually when you want **custom real-time logic**.

```csharp
HubConnection connection = new HubConnectionBuilder()
    .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
    .WithAutomaticReconnect()
    .Build();

await connection.StartAsync();
```

This establishes a persistent real-time connection to the hub.

---

## 7️⃣ Sending Messages

To call a method on the hub:

```csharp
await connection.SendAsync("SendMove", position, 'X');
```

This:
- Calls `SendMove` on the server
- Executes server logic
- Can trigger broadcasts to other clients

---

## 8️⃣ Receiving Messages

Clients subscribe to messages using `On`:

```csharp
connection.On<int, char>("ReceiveMove", (position, symbol) =>
{
    // Update local UI state
});
```

Key idea:
- Method names are string-based
- Signatures must match
- UI updates must trigger `StateHasChanged()`

---

## 9️⃣ Managing Multiple Players

SignalR identifies clients using:
- `Context.ConnectionId`

Typical multiplayer patterns:
- Assign roles based on connection order (X / O)
- Group clients into rooms (game IDs)
- Allow spectators
- Reject invalid actions (not your turn, game over, etc.)

The **server should always validate actions**.

---

## 🔌 Handling Disconnects

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

Server side:

```csharp
public override Task OnDisconnectedAsync(Exception? exception)
{
    // Cleanup player state
    return base.OnDisconnectedAsync(exception);
}
```

---

## 🎮 Applying This to a Game

In a multiplayer game:
- Clients send **intent** (e.g. “play at position 5”)
- Server validates the move
- Server updates the game state
- Server broadcasts the updated state to all players

Clients never modify the authoritative state directly.

---

## ✅ Wrap-Up

By the end of this lesson, you understand:

- What SignalR is and why it exists
- How SignalR fits into Blazor Server
- How to define and register a hub
- How clients send and receive messages
- How SignalR enables multiplayer and collaboration

This knowledge applies to:
- Games
- Chats
- Dashboards
- Any real-time feature

**Next lesson:** Applying SignalR to our Tic-Tac-Toe game and building a real multiplayer experience. 🎮
