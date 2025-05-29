# 🏠 Homework – Lesson 8: Enable Real-Time Multiplayer

## 🎯 Objective

Enable real-time communication between two players using SignalR.

---

## 📌 Tasks

### 1. Set up a SignalR Hub

- Define a method to broadcast moves
- Map the hub in `Program.cs`

---

### 2. Connect your Blazor client

- Use `HubConnectionBuilder` to connect
- Handle `ReceiveMove` to update the board when opponent plays

---

### 3. Send a move

- When a player makes a move, send it through SignalR
- Ensure the move appears on the opponent's board

---

### 4. Handle disconnects

- Display a message when the other player leaves
- Reconnect automatically if disconnected

---

### 5. Bonus Challenges

- Indicate whose turn it is
- Disable clicks on the board when it’s not your turn
- Add a rematch button

---

## 📦 Submission

- Push your updated multiplayer Blazor app to GitHub
- Ensure at least two browser tabs can play against each other

---

Well played, team player! 🧑‍🤝‍🧑
