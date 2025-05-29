# 🏠 Homework

## 🎯 Objective

Build a basic interactive console-based Tic-Tac-Toe playground. Practice working with arrays, user input, error handling, and loops.

---

## 📌 Required Tasks

### 1. Display a 3x3 Tic-Tac-Toe board

- Use a 2D array to represent the board.
- Print the board like this:

```
 1 | 2 | 3
---|---|---
 4 | 5 | 6
---|---|---
 7 | 8 | 9
```

---

### 2. Handle player input

- Ask the player to enter a number between `1` and `9`.
- If the input is valid and the cell is not already taken, place an `'X'` there.
- If the input is invalid (non-number, out of range, or taken), show an error message and prompt again.

---

### 3. Repeat until finished

- Continue asking for input until the board is full.
- Also allow the user to type `"exit"` to quit the game early.

---

## 🧩 Bonus Challenges

- Let the player choose between `'X'` and `'O'`.
- Alternate turns between two players.
- Detect when the board is full and end the game.
- Separate responsibilities using methods:
  - `PrintBoard()`
  - `IsValidMove()`
  - `MakeMove()`

---

## 💡 Tips

- Use `int.TryParse()` to safely parse user input.
- Store the board as a `char[,]` array.
- Use `for` loops to draw the board dynamically.
- Validate edge cases to prevent unexpected behavior.

Good luck and have fun! 🎮
