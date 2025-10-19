# 🏠 Homework – Lesson 5: Building & Using a Web API

## 🎯 Objective

Use your Web API to play TicTacToe through a simple web interface. Learn to send/receive JSON, update the UI dynamically, and handle server errors.

---

## 📌 Tasks

### 1. Build the basic frontend

- Create a Razor Page or static HTML page that:
  - Displays the board using HTML buttons or divs
  - Uses JavaScript to call your API via `fetch()`

---

### 2. Connect to your API

- On each button click, send a `POST` request to `/api/game/move`
- After each move:
  - Check if the game is over
  - Update the board and show a message

---

### 3. Add reset functionality

- Add a “Reset Game” button that calls `POST /api/game/reset`
- Clear the board and reset game state

---

## 🧩 Bonus Challenges

- Add an endpoint to get the overall game stats
- Add keyboard shortcuts or accessibility improvements

---

## 📦 Submission

- Push your Web API project and HTML frontend to GitHub
- Make sure it builds and runs
- Include screenshots or a short explanation in a `README.md` file

---

You’re officially building full-stack apps now — great job! 🚀
