# 🧰 Preparation – Lesson 6: Storing Games in a Database

## ✅ Goals for this lesson

This week, you’ll:

- Understand the role of databases in persistent data storage
- Learn how to use SQL Server with C#
- Explore manual database access using ADO.NET
- Discover Entity Framework Core (EF Core) as a modern alternative
- Compare ADO.NET vs EF Core
- Store game data in the database

---

## 🧠 Before You Arrive

Refresh your understanding of:

- Basic SQL syntax: `SELECT`, `INSERT`, `UPDATE`, `DELETE`
- What a relational database is
- The difference between in-memory and persistent storage

No prior experience with ADO.NET or EF Core is required.

---

## 🛠 Setup Checklist

Make sure the following are installed and working:

- ✅ SQL Server (choose one):
  - [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Windows)
  - [SQL Server Docker image](https://hub.docker.com/_/microsoft-mssql-server) (Mac/Linux)
- ✅ A SQL client:
  - [SQL Server Management Studio (SSMS)](https://aka.ms/ssms)
  - or [Azure Data Studio](https://learn.microsoft.com/en-us/sql/azure-data-studio/)
- ✅ .NET 9 SDK – [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- ✅ Rider IDE – Already installed from previous lessons

Test your setup:

- Run a console app that successfully connects to a SQL Server instance
- Run the following SQL query to verify access:

```sql
SELECT GETDATE();
```

---

## 💬 Think About

- What kind of data from your game is worth saving? (Moves, players, rounds, etc.)
- What are the tradeoffs between full control (ADO.NET) and abstraction (EF Core)?
- In which situations would you prefer writing raw SQL yourself?

---

See you in class! 👋
