# 🧰 Preparation – Lesson 7: Introducing Blazor for UI

## ✅ Goals for this lesson

This week, you’ll:

- Understand what Blazor is and how it fits into the .NET ecosystem
- Build interactive user interfaces using Blazor Server
- Use data binding and event handling in components
- Consume a Web API using `HttpClient` in Blazor
- Prepare the foundation for a multiplayer Tic Tac Toe game

---

## 🧠 Before You Arrive

Make sure you’re familiar with:

- The basics of HTTP and APIs (from the previous lessons)
- The structure of the TicTacToe game logic in `TicTacToe.Core`
- Razor syntax (similar to HTML with embedded C#)

No prior knowledge of Blazor is required, but being comfortable with MVC and Web API concepts will help.

---

## 🛠 Setup Checklist

Please ensure the following tools are available and working:

- ✅ Rider IDE (latest version)
- ✅ .NET 9 SDK

In addition, prepare the following:

- A solution that contains:
  - `TicTacToe.Core` (shared logic)
  - `TicTacToe.Api` (Web API)
  - A new Blazor Server project: `TicTacToe.Blazor`

To create the Blazor project and link it up:

```bash
dotnet new blazorserver -n TicTacToe.Blazor
dotnet add TicTacToe.Blazor/TicTacToe.Blazor.csproj reference TicTacToe.Core/TicTacToe.Core.csproj
dotnet add TicTacToe.Blazor/TicTacToe.Blazor.csproj reference TicTacToe.Api/TicTacToe.Api.csproj
```

Open the solution in Rider and ensure the Blazor project runs successfully with a default template.

---

## 💬 Think About

- What are the benefits of writing the UI in C# rather than JavaScript?
- What does a Blazor component represent?
- How would you structure the UI logic to remain modular and testable?

See you in class! 🚀
