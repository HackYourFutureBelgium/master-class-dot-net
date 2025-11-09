# Lesson 6: Storing Games in a Database

## 📌 Lesson Overview
- Why use a database?
- Setting up SQL Server
- The classic ADO.NET way (manual SQL)
- The modern Entity Framework Core way (ORM)
- Code First vs Database First
- EF configuration: Attributes vs Fluent API
- Creating and applying migrations
- Comparing ADO.NET and EF Core
- Database error handling

---

## 1️⃣ Why Use a Database?

- A database allows data to persist between sessions — once your app closes, the data remains available.
- We’ll use **SQL Server**, a powerful **Relational Database Management System (RDBMS)** that integrates seamlessly with .NET.

---

## 2️⃣ Setting Up SQL Server

Install one of:
- 🖥 **SQL Server Developer Edition** (Windows)
- 🐳 **SQL Server Docker image** (Mac/Linux)

Tools:
- **SQL Server Management Studio (SSMS)** (Windows)
- **Azure Data Studio** or **JetBrains DataGrip** (cross-platform)

---

## 3️⃣ ADO.NET – The Classic Way

ADO.NET is the traditional way to interact with SQL databases in .NET.  
It requires you to manually write SQL commands and handle connections, readers, and parameters.

---

### ✅ Step 1: Create the Table (SQL Script)

```sql
CREATE DATABASE TicTacToeDB;
GO

USE TicTacToeDB;
GO

CREATE TABLE Moves (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Position INT NOT NULL,
    Symbol CHAR(1) NOT NULL
);
```

---

### ✅ Step 2: Connecting to the Database

```csharp
using Microsoft.Data.SqlClient; // or System.Data.SqlClient

string connectionString = "Server=localhost;Database=TicTacToeDB;Trusted_Connection=True;";
```

---

### ✅ Step 3: Inserting Data

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

### ✅ Step 4: Reading Data

```csharp
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();
    string query = "SELECT Id, Position, Symbol FROM Moves";

    using (SqlCommand command = new SqlCommand(query, connection))
    using (SqlDataReader reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            int position = reader.GetInt32(1);
            char symbol = reader.GetString(2)[0];

            Console.WriteLine($"Move {id}: {symbol} at {position}");
        }
    }
}
```

💡 *ADO.NET gives you full control, but requires lots of repetitive code.*

---

## 4️⃣ Entity Framework Core – The Modern Way

EF Core is an **Object-Relational Mapper (ORM)**.  
It translates C# classes into database tables and lets you query data using LINQ instead of SQL.

---

### ✅ Step 1: Install EF Core

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

### ✅ Step 2: Create the Model and DbContext

```csharp
using Microsoft.EntityFrameworkCore;

public class Move
{
    public int Id { get; set; }
    public int Position { get; set; }
    public char Symbol { get; set; }
}

public class TicTacToeDbContext : DbContext
{
    public DbSet<Move> Moves { get; set; }

    public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=TicTacToeDB;Trusted_Connection=True;");
    }
}
```

---

### ✅ Step 3: Add and Save Data

```csharp
using var db = new TicTacToeDbContext();

db.Moves.Add(new Move { Position = 5, Symbol = 'X' });
await db.SaveChangesAsync();
```

---

### ✅ Step 4: Read Data

```csharp
var moves = await db.Moves.ToListAsync();

foreach (var move in moves)
{
    Console.WriteLine($"Move {move.Id}: {move.Symbol} at {move.Position}");
}
```

---

## 5️⃣ Code First vs Database First

| Approach | Description | When to Use |
|-----------|--------------|-------------|
| **Code First** | You create C# classes and EF creates the database schema. | When starting from scratch. |
| **Database First** | You start from an existing database and generate C# models. | When integrating an existing database. |

---

## 6️⃣ Configuring EF Core

### 🧩 Using Attributes

```csharp
public class Move
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int Position { get; set; }

    [Column(TypeName = "char(1)")]
    public char Symbol { get; set; }
}
```

---

### 🧩 Using Fluent API (inside `OnModelCreating`)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Move>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Symbol).HasColumnType("char(1)").IsRequired();
        entity.Property(e => e.Position).IsRequired();
    });
}
```

---

### 🧩 Using Fluent API in Separate Configuration Class

```csharp
public class MoveConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Position).IsRequired();
        builder.Property(e => e.Symbol).HasColumnType("char(1)");
    }
}
```

Then register it in your context:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new MoveConfiguration());
}
```

---

## 7️⃣ Creating and Applying Migrations

### Step 1: Enable EF Tools
Make sure your project has EF CLI tools:
```bash
dotnet tool install --global dotnet-ef
```

### Step 2: Create the Migration
```bash
dotnet ef migrations add InitialCreate
```

### Step 3: Apply the Migration (create database + tables)
```bash
dotnet ef database update
```

This generates a `Migrations` folder containing migration files and creates your SQL schema automatically.

---

## 8️⃣ ADO.NET vs EF Core – Comparison Table

| Feature             | ADO.NET               | EF Core                            |
|---------------------|-----------------------|------------------------------------|
| SQL writing         | Manual                | Automatic (via LINQ or migrations) |
| Object mapping      | Manual                | Automatic (`DbSet`)                |
| Connection handling | Manual                | Automatic                          |
| Query flexibility   | Full SQL control      | LINQ abstraction                   |
| Speed               | Very fast (low-level) | Slight overhead                    |
| Complexity          | More verbose          | Simpler, declarative               |
| Schema evolution    | Manual                | Migrations supported               |

---

## 9️⃣ Database Error Handling

### EF Core
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

### ADO.NET
```csharp
try
{
    command.ExecuteNonQuery();
}
catch (SqlException ex)
{
    Console.WriteLine($"SQL error: {ex.Message}");
}
```

---

## 🚀 End of Lesson 6
