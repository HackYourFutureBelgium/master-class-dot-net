# Lesson 6: Storing Games in a Database

## 📌 Lesson Overview
- Why use a database?
- Setting up SQL Server
- Introduction to ADO.NET (manual database access)
- Introduction to Entity Framework Core (modern ORM)
- Comparing ADO.NET and EF Core
- Storing and retrieving game data
- Database error handling

---

## 1️⃣ Why Use a Database?

- Databases allow us to store data permanently across sessions.
- SQL Server is a relational database management system (RDBMS) that works well with .NET.

---

## 2️⃣ Setting Up SQL Server

Install one of:
- SQL Server Developer Edition (Windows)
- SQL Server Docker image (Mac/Linux)

Tools:
- **SQL Server Management Studio (SSMS)** or **Azure Data Studio**

---

## 3️⃣ ADO.NET – The Classic Way

ADO.NET allows direct execution of SQL queries.

### Connecting to the Database

```csharp
using System.Data.SqlClient;

string connectionString = "Server=localhost;Database=TicTacToeDB;Trusted_Connection=True;";
```

### Inserting a Move

```csharp
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    string query = "INSERT INTO Moves (Position, Symbol) VALUES (@Position, @Symbol)";

    using (SqlCommand command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@Position", position);
        command.Parameters.AddWithValue("@Symbol", symbol);
        command.ExecuteNonQuery();
    }
}
```

---

## 4️⃣ Entity Framework Core – The Modern Way

EF Core is an Object-Relational Mapper (ORM).

### Setting Up EF Core

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

### Creating a DbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class TicTacToeDbContext : DbContext
{
    public DbSet<Move> Moves { get; set; }

    public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options)
        : base(options) { }
}

public class Move
{
    public int Id { get; set; }
    public int Position { get; set; }
    public char Symbol { get; set; }
}
```

---

### Using the DbContext

```csharp
var move = new Move { Position = 5, Symbol = 'X' };
dbContext.Moves.Add(move);
await dbContext.SaveChangesAsync();
```

---

## 5️⃣ ADO.NET vs EF Core – Comparison Table

| Feature            | ADO.NET                     | EF Core                  |
|--------------------|------------------------------|---------------------------|
| SQL writing        | Manual                       | Automatic (can override)  |
| Object Mapping     | Manual (`SqlDataReader`)      | Automatic (`DbSet`)        |
| Connection handling| Manual (`Open`, `Close`)      | Automatic                 |
| Query Flexibility  | Full control                 | High-level abstraction    |
| Complexity         | More code, very detailed     | Less code, declarative    |
| Speed              | Slightly faster (low-level)  | Very good, small overhead |

---

## 6️⃣ Database Error Handling

Example with EF Core:

```csharp
try
{
    await dbContext.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    Console.WriteLine($"Database error: {ex.Message}");
}
```

In ADO.NET, catch `SqlException`.

---

## 🚀 End of Lesson 6
