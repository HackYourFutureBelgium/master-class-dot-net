# 🏠 Homework – Lesson 6: Save Moves to a Database

## 🎯 Objective

Use either ADO.NET or EF Core to save your TicTacToe game moves into a database.

---

## 📌 Tasks

### 1. Create the database

- Database name: `TicTacToeDB`
- Table: `Moves`
  - `Id` (int, identity)
  - `Position` (int)
  - `Symbol` (char)

---

### 2. Insert moves into the database

Each time a player makes a move:
- Save the position and symbol in the `Moves` table.

---

### 3. Retrieve moves

After the game ends:
- Query the database and list all moves played.

---

## 🧩 Bonus Challenges

- Track full games (who won, when played).
- Separate games and moves into two tables with foreign keys.

---

## 📦 Submission

- Push updated project files to GitHub
- Include a `.sql` file or migration script if you created the database manually

---

Solid persistence work! 🚀
