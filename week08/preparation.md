# 🧰 Preparation – Lesson 8: Multiplayer with SignalR

## ✅ Goals for this lesson

This week, you’ll:

- Understand what SignalR is and when to use it
- Set up a SignalR Hub on the server (API)
- Connect your Blazor app to the SignalR hub
- Send and receive real-time messages between clients
- Coordinate multiplayer turns using client-server communication

---

## 🧠 Before You Arrive

Make sure you’ve reviewed:

- Basics of Blazor components (from Lesson 7)
- What a WebSocket is (basic idea of server push communication)
- The concept of client-server roles in multiplayer apps

---

## 🛠 Setup Checklist

You’ll need the following NuGet packages installed before class:

✅ In **TicTacToe.Web**:
```bash
dotnet add package Microsoft.AspNetCore.SignalR
dotnet add package Microsoft.AspNetCore.SignalR.Client
```

## 💬 Think About

- How should turns be managed so two players can’t play at the same time?
- What would happen if a player disconnects mid-game?

See you in class!
