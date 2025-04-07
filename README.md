# .NET Master Class

Welcome to the HackYourFuture .NET Master Class — a 10-week intensive journey into full-stack development with C# and the .NET ecosystem. This module is designed for students who already have a foundation in programming (especially JavaScript and React) and want to take their skills to the next level by mastering a strongly typed, object-oriented language, server-side architecture, and modern frontend/backend integration techniques.

Over the course of this master class, you will:
- Learn the core features of the C# programming language.
- Build a game step-by-step, starting from a console application and evolving into a full-stack web app.
- Explore the .NET ecosystem, including MVC, Web API, Entity Framework Core, Blazor, and SignalR.
- Apply software engineering practices like clean architecture, error handling, input validation, and separation of concerns.
- Develop a working multiplayer game with real-time communication and persistent storage.

By the end of the module, you'll be equipped with the skills needed to:
- Build and structure modern C# applications.
- Understand and apply object-oriented design principles.
- Create web APIs and connect them to rich frontend clients.
- Use relational databases with SQL Server and Entity Framework Core.
- Handle real-time communication using SignalR and Blazor.

## Prerequisites

To make the most of this course, students should:

- ✅ Be comfortable with basic programming concepts (variables, loops, conditionals, functions).
- ✅ Have experience writing JavaScript (React knowledge is a plus).
- ✅ Understand how HTTP works and what a REST API is at a high level.
- ✅ Be familiar with version control using Git and GitHub.
- ✅ Have a working development environment (VS Code, Node, browser dev tools).

No previous experience with C# or .NET is required — we will cover it from the ground up.

## Preparation

Before the first session, please:

1. 🛠 Install the necessary tools (see below).
2. ✏️ Make sure you can open and run a basic console app in Rider.

We’ll help during the first session if you run into any issues, but the smoother your setup, the more time we can spend learning!

## Environment setup
IDE: *Rider*, *Rider* is a cross-platform .NET IDE that is used by many .NET developers. It is available for Windows, Mac, and Linux. You can download it from the [JetBrains website](https://www.jetbrains.com/rider/), it is free for non-commercial use.

RDBMS: *Microsoft SQL Server*, *SQL Server* is one of the most used relational database management system in the realm of .NET development. If you are a *Windows* user, you can download the **Developer edition** of *SQL Server* from the [Microsoft website](https://www.microsoft.com/en-gb/sql-server/sql-server-downloads) (or using this [direct link](https://go.microsoft.com/fwlink/p/?linkid=2215158&clcid=0x809&culture=en-gb&country=gb)). If you are a Mac or Linux user, you can use the Docker image of *SQL Server*. You can find the instructions on how to run the Docker image of *SQL Server* on the [Microsoft website](https://hub.docker.com/_/microsoft-mssql-server).

## Planning
### 🟢 Phase 1: C# Foundations & Console Game

#### Lesson 1: Introduction to C# & Building the Console Game
([Preparation](week1/preparation.md), [Lesson plan](week1/lesson-plan.md), [Homework](week1/homework.md))
- Introduction to C# and .NET
- Writing a simple console application
- Data Types & Variables (strings, integers, booleans, etc.)
- String manipulation (concatenation, `string.Format()`, interpolation)
- Arrays (single and multidimensional)
- Loops (`for`, `while`, `foreach`)
- Handling user input and output
- Basic error handling** (try-catch)
- Understanding stack vs. heap memory

#### Lesson 2: Object-Oriented Programming & Improving the Game
([Preparation](week2/preparation.md), [Lesson plan](week2/lesson-plan.md), [Homework](week2/homework.md))
- Object-oriented programming in C#
- Encapsulation using access modifiers and properties
- Structuring the game with `Board`, `Player`, and `GameController` classes
- Game state management using enums
- Defensive programming and input validation
- Game loop for multiple rounds

#### Lesson 3: Collections, LINQ & Structuring Code
([Preparation](week3/preparation.md), [Lesson plan](week3/lesson-plan.md), [Homework](week3/homework.md))
- Collections in C#: Using lists, arrays for board representation.
- Introduction to LINQ:
  - Checking win conditions efficiently.
- Singleton Pattern:
  - Classic vs. modern ASP.NET Core DI approach.
- Extracting Logic into a Class Library:
  - Moving `GameService` into a separate project.
- ⚠️ Logging Errors: Using `try-catch` blocks and logs.

### 🟠 Phase 2: Web Development & API Integration

#### Lesson 4: Introducing MVC & Web Applications
([Preparation](week4/preparation.md), [Lesson plan](week4/lesson-plan.md), [Homework](week4/homework.md))
- MVC Basics: Controllers, Models, Views.
- Building the Game in MVC:
  - Displaying the board in a Razor View.
  - Handling user moves via forms.
- ⚠️ Handling API Errors:
  - Validating requests in controllers.
  - Returning meaningful error messages.

#### Lesson 5: Creating a Web API & Consuming It
([Preparation](week5/preparation.md), [Lesson plan](week5/lesson-plan.md), [Homework](week5/homework.md))
- Intro to Web APIs:
  - REST principles.
- Exposing Game Logic via API:
  - `POST /api/game/move`
  - `GET /api/game/state`
- Consuming API from MVC:
  - Using JavaScript/Fetch.
- ⚠️ API Error Handling:
  - Custom error responses.
  - HTTP status codes (`400`, `500`).

### 🔵 Phase 3: Database, Blazor & Multiplayer

#### Lesson 6: Storing Games in a Database
([Preparation](week6/preparation.md), [Lesson plan](week6/lesson-plan.md), [Homework](week6/homework.md))
- Using ADO.NET vs. EF Core:
  - Basic SQL queries.
  - Migrating to EF Core.
- Storing Game History:
  - Saving moves & results.
- Querying Past Games: Fetching previous results.
- ⚠️ Database Exception Handling:
  - Catching SqlException errors.
  - Handling connection issues.

#### Lesson 7: Introducing Blazor for UI
([Preparation](week7/preparation.md), [Lesson plan](week7/lesson-plan.md), [Homework](week7/homework.md))
- Why Blazor?:
  - Comparing to MVC.
- Building a Blazor UI:
  - Interactive board rendering.
  - Fetching data from the API.
- ⚠️ UI Error Handling:
  - Handling API failures.
  - Preventing invalid user actions.

#### Lesson 8: Multiplayer with SignalR
([Preparation](week8/preparation.md), [Lesson plan](week8/lesson-plan.md), [Homework](week8/homework.md))
- Introduction to SignalR:
  - Real-time game updates.
- Building Online Multiplayer:
  - Updating board live.
  - Handling real-time player moves.
- ⚠️ SignalR Error Handling:
  - Handling disconnects.
  - Preventing message loss.

### 🚀 Final phase: Polishing & Optimization

#### Lesson 9: Improving Performance & Game Logic
([Preparation](week9/preparation.md), [Lesson plan](week9/lesson-plan.md), [Homework](week9/homework.md))
- Optimizing Queries:
  - Using LINQ efficiently.
  - Preventing redundant API calls.
- Refactoring Code:
  - Cleaning up the GameService.
- ⚠️ Debugging Best Practices:
  - Using Rider debugging tools.

#### Lesson 10: Final Enhancements & Closing
([Preparation](week10/preparation.md), [Lesson plan](week10/lesson-plan.md), [Homework](week10/homework.md))
- Extending the Game:
  - Adding a scoring system.
  - Supporting AI opponents.
- Review & Q&A:
  - Summary of key concepts.
  - Suggestions for further learning.
