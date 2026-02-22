# 🏠 Homework – Lesson 7: Build a Blazor UI for Your Game

## 🎯 Objective

Build a fully interactive Tic Tac Toe UI in Blazor Server, wired to the `GameEngine` from `TicTacToe.Core`.

---

## 📌 Tasks

### 1. Set up the project and DI

- Create a `TicTacToe.Web` Blazor Server project referencing `TicTacToe.Core`
- Register `GameStatsService` and `GameEngine` as singletons in `Program.cs`
- Add a `🎮 Game` nav link in `NavMenu.razor` pointing to `/game`

---

### 2. Build the game setup form

Create a `Game.razor` page at `/game` that:

- Collects Player 1 name, Player 2 name, and board size (3–9) using plain `<input>` elements with `@bind`
- Calls `Engine.SetPlayers(...)` and `Engine.SetBoardSize(...)` when the form is submitted
- Shows the board only once the game has started

---

### 3. Display the board and handle moves

- Render the board as a grid of buttons using `@for` loops
- Each button shows the cell value (`X`, `O`, or empty)
- Clicking a cell calls `Engine.TryPlayMove(position)`
- Disable cells that are already filled or when the game is over

---

### 4. Create a `Cell` component

Extract each cell button into a reusable `Cell.razor` component with:

- `Value` (`char`) — the symbol to display
- `Index` (`int`) — the position (1-based)
- `Disabled` (`bool`) — whether clicking is allowed
- `OnClicked` (`EventCallback<int>`) — raised when the cell is clicked

---

### 5. Create a `GameBoard` component

Extract the board grid into a `GameBoard.razor` component with:

- `Board` (`Board`) — the board to render
- `IsInProgress` (`bool`) — controls cell interactivity
- `OnCellClicked` (`EventCallback<int>`) — forwarded from `Cell`

Use it in `Game.razor`:

```razor
<GameBoard Board="@Board"
           IsInProgress="@(Engine.Status == GameStatus.InProgress)"
           OnCellClicked="MakeMove" />
```

---

### 6. Show game outcome and allow a new round

After a win or draw:

- Display a win/draw message
- Show a scoreboard with each player's win count
- Show the move history
- Provide a button to play another round (with a configurable board size)
- Provide a button to replay the same size

---

## 🏆 Bonus

- Add an **Undo** button that calls `Engine.TryUndoLastMove()`
- Style the board using Bootstrap button classes
- Display a different badge colour for the current player depending on their symbol

---

Great job making the leap into interactive web UIs! 🎨
