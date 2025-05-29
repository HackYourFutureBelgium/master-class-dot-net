# 🏠 Homework

## 🧩 Assignment: Mini Tic-Tac-Toe Playground

Create a simple console-based Tic-Tac-Toe playground where a player can choose a position to place an `'X'`.

### 📝 Requirements

1. Display a 3x3 Tic-Tac-Toe board using a 2D array:
    ```
     1 | 2 | 3~~~~
    ---|---|---
     4 | 5 | 6
    ---|---|---
     7 | 8 | 9
    ```

2. Ask the player to enter a number between `1` and `9`.

3. If the input is valid and the position is not taken, replace the number with `'X'`.

4. If the input is invalid (non-number, out of range, or already taken), show an error message and ask again.

5. Continue until the board is full or the player types `"exit"`.

### Example Output:
```
Current board:
 1 | 2 | 3
---|---|---
 4 | 5 | 6
---|---|---
 7 | 8 | 9

Enter position (1-9) or type 'exit': 5

Current board:
 1 | 2 | 3
---|---|---
 4 | X | 6
---|---|---
 7 | 8 | 9
```

---

## 🚀 Bonus Challenges (Optional)

- Let the player choose between `'X'` and `'O'`.
- Alternate turns between two players.
- Detect when the board is full and end the game.
- Separate your code into methods like `PrintBoard()`, `IsValidMove()`, and `MakeMove()`.

---

## 💡 Tips

- Use `int.TryParse()` to validate numeric input.
- Use a `char[,]` array for the board.
- Use nested `for` loops to print the grid.
- Test your code thoroughly!

Good luck and have fun! 🎮
