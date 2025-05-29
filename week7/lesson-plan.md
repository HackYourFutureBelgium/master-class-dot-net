# Lesson 7: Introducing Blazor for UI

## 📌 Lesson Overview

In this lesson, you’ll learn how to build a user interface for your TicTacToe game using Blazor Server.

---

## 1️⃣ What is Blazor?

- A UI framework for building interactive web apps using C# instead of JavaScript.
- Two hosting models:
  - **Blazor Server** (runs on the server, updates UI via SignalR)
  - **Blazor WebAssembly** (runs in the browser)
- We'll use **Blazor Server** in this course.

---

## 2️⃣ Project Structure

We'll create a new project:

```bash
dotnet new blazorserver -n TicTacToe.Blazor
```

Add reference to:
- `TicTacToe.Core` (game logic)
- `TicTacToe.Api` (if you want to call your Web API)

---

## 3️⃣ Blazor Basics

Blazor uses components to build UIs.

Example component:

```razor
<h3>Hello @Name</h3>

@code {
    [Parameter]
    public string Name { get; set; }
}
```

### Data Binding

```razor
<input @bind="playerName" />
<p>Hello, @playerName!</p>

@code {
    private string playerName;
}
```

### Event Handling

```razor
<button @onclick="MakeMove">Click me</button>

@code {
    void MakeMove()
    {
        Console.WriteLine("Move played");
    }
}
```

---

## 4️⃣ Building the Tic Tac Toe UI

### Creating the Board

```razor
@for (int i = 0; i < 9; i++)
{
    <button @onclick="() => PlayMove(i)">
        @board[i]
    </button>
}

@code {
    string[] board = new string[9];

    void PlayMove(int index)
    {
        if (board[index] == null)
        {
            board[index] = "X";
        }
    }
}
```

---

## 5️⃣ Fetching Game State from an API

In `Program.cs`:

```csharp
builder.Services.AddHttpClient();
```

In component:

```razor
@inject HttpClient Http

@code {
    protected override async Task OnInitializedAsync()
    {
        var state = await Http.GetFromJsonAsync<GameState>("api/game/state");
        // Populate UI from state
    }
}
```

---

## 6️⃣ UI Error Handling

- Show an error message if the API is unavailable:

```csharp
try
{
    var result = await Http.PostAsJsonAsync("api/game/move", move);
}
catch (Exception ex)
{
    errorMessage = "Could not contact server.";
}
```

- Display errors using `if` blocks:

```razor
@if (!string.IsNullOrEmpty(errorMessage))
{
    <div class="alert alert-danger">@errorMessage</div>
}
```

---

## 7️⃣ Bonus: Styling the Board

Use CSS to make the board look nice:

```css
button {
    width: 50px;
    height: 50px;
    margin: 2px;
    font-size: 20px;
}
```

---

## 8️⃣ Wrap-up

In this lesson, you learned to:
- Create a Blazor component to display and interact with a Tic Tac Toe board
- Bind and update state in the UI
- Fetch data from your API using `HttpClient`
- Handle errors gracefully in the UI

Next time: **SignalR & Multiplayer**! 🎮
