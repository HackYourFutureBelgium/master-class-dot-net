# 🧰 Preparation – Lesson 7: Introducing Blazor for UI

## ✅ Goals for this lesson

This week, you'll:

- Understand what Blazor is and how it fits into the .NET ecosystem
- Build interactive user interfaces using Blazor Server
- Use data binding and event handling in components
- Consume game logic from `TicTacToe.Core` directly in a Blazor component
- Prepare the foundation for a multiplayer Tic Tac Toe game

---

## 🧠 Before You Arrive

Make sure you're familiar with:

- The structure of the TicTacToe game logic in `TicTacToe.Core`
- Razor syntax (similar to HTML with embedded C#)

No prior knowledge of Blazor is required, but being comfortable with MVC and Razor Views concepts will help.

---

## 🛠 Setup Checklist

Please ensure the following tools are available and working:

- ✅ Rider IDE (latest version)
- ✅ .NET 9 SDK

In addition, prepare the following:

- A solution that contains:
  - `TicTacToe.Core` (shared game logic)
  - A new Blazor Server project: `TicTacToe.Web`

To create the Blazor project and link it up:

```bash
dotnet new blazor -n TicTacToe.Web
dotnet add TicTacToe.Web/TicTacToe.Web.csproj reference TicTacToe.Core/TicTacToe.Core.csproj
```

Open the solution in Rider and ensure the Blazor project runs successfully with a default template.

---

## 💬 Think About

- What are the benefits of writing the UI in C# rather than JavaScript?
- What does a Blazor component represent?
- How would you structure the UI logic to remain modular and testable?

See you in class! 🚀
