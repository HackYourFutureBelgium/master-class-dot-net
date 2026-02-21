# 🏠 Homework – Lesson 8: Connection-Exclusive Player Roles

## 🎯 Objective

Right now, any number of connections can join a game room and any of them can make moves. Your task is to make the game **connection-exclusive**: the first two connections to a room become Player X and Player O, and everyone else is a spectator.

Only the player whose turn it is should be able to interact with the board.

---

## 📌 Tasks

### 1. Assign a role when joining

In `GameHub`, track which connection IDs are the two players for each game room. When a connection calls `JoinGame`:

- If the room has no Player X yet, assign this connection as Player X
- If the room has no Player O yet, assign this connection as Player O
- Otherwise, assign this connection as a Spectator

Tell the caller which role they received:

```csharp
await Clients.Caller.SendAsync("RoleAssigned", role); // "X", "O", or "Spectator"
```

---

### 2. Store the role in the component

In `Game.razor`, subscribe to `"RoleAssigned"` and store the value in a private field:

```csharp
private string _role = "Spectator";
```

---

### 3. Enforce turn-based access

Before making a move, check that:
- This connection is a player (not a spectator)
- It is actually their turn

```csharp
private async Task MakeMove(int position)
{
    if (_role == "Spectator") return;
    if (Engine.CurrentPlayer.Symbol != (_role == "X" ? 'X' : 'O')) return;

    Engine.TryPlayMove(position);
    await BroadcastUpdate();
}
```

---

### 4. Show the role in the UI

Display a small badge so each user knows who they are:

```razor
<p>You are: <span class="badge bg-secondary">@_role</span></p>
```

---

### 5. Clean up on disconnect

In `GameHub`, override `OnDisconnectedAsync` to release the player's slot when they leave. This allows a new connection to take their place.

---

## 🏆 Bonus

- Show a "Waiting for opponent…" message when only one player has joined
- Disable the board visually (not just via logic) when it is not your turn
- Allow only Player X to configure and start the game (set player names, board size)

---

Good luck, and may the best connection win! 🎮
