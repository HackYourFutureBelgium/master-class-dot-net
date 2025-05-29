# 🏠 Homework

## 🎯 Objective

Practice using ASP.NET Core MVC by connecting your game logic to a web interface and handling user interactions through forms and controllers.

---

## 📌 Tasks

### 1. Display player names

- Add input fields to collect names before the game starts.
- Show the player names during the game and in the "Game Over" screen.
- Use `TempData`, `ViewBag`, or a model to pass names between views.

---

### 2. Track and display win count

- Create a `Player` class that includes:
  - `Name`, `Symbol`, and `Wins` count
- Store both players in a static or session-scoped object
- Increment the winner's score after each game
- Display a scoreboard on the page

---

### 3. Improve the end screen

- Show a message like:  
  `🎉 Alice (X) wins!` or `🤝 It's a draw!`
- Show the current win count for both players
- Include a button: “Play Again”

---

## 🧩 Bonus Challenges

- Store player state in `HttpContext.Session` or via a service class
- Let players choose their symbol (X or O) before the game starts
- Add a "Reset Game" button that clears all progress

---

Good luck and have fun! 🎮
