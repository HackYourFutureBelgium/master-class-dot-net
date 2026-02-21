# Lesson 7: Introducing Blazor for UI

## 📌 Lesson Overview

In this lesson, you’ll build a stateful user interface for your TicTacToe game using Blazor Server. You’ll connect what you already know about MVC and Razor Views to Blazor components, then handle user interactions with C#.

---

## ✅ Prerequisites (from earlier lessons)

- C# basics, collections, and classes (weeks 1–3)
- MVC and Razor Views (week 4)
- Web API basics (week 5, optional for this lesson)

---

## 1️⃣ What is Blazor?

- A UI framework for building interactive web apps using C# instead of JavaScript.
- Two hosting models:
  - **Blazor Server** (runs on the server, updates UI via SignalR)
  - **Blazor WebAssembly** (runs in the browser)
- We'll use **Blazor Server** in this course.

---

## 2️⃣ Bridge from MVC/Razor to Blazor

What you already know (MVC):
- Razor Views render HTML **per request**.
- User actions typically trigger **controller actions** and **full page reloads**.

What is new in Blazor:
- Razor Components stay **stateful** on the server.
- UI updates happen when **component state changes**, without full reloads.
- Events (like button clicks) call **C# methods directly**.

---

## 3️⃣ Project Structure

We'll create a new project:

```bash
dotnet new blazor -n TicTacToe.Web
```

Add reference to:
- `TicTacToe.Core` (game logic)

Where things live:
- `Components/Pages/` for routed pages
- `Components/Layout/` for layout components
- `Components/` for reusable components
- `wwwroot/` for static assets

---

## 4️⃣ Routing and Navigation

A component becomes a page when it has a route:

```razor
@page "/game"
```

Navigation options:
- `<NavLink>` for menu links (active styling built in)
- `NavigationManager` for redirects in code

Example:

```razor
@inject NavigationManager Nav

<button @onclick="() => Nav.NavigateTo("/game")">Start</button>
```

---

## 5️⃣ Blazor Basics (Razor Syntax Refresher)

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

### When to use `@` in Razor

Use `@` when you want **C# inside markup**:
- Variables and expressions: `@playerName`, `@(score + 1)`
- Control flow: `@if`, `@for`, `@foreach`, `@switch`
- Event handlers: `@onclick="MakeMove"`
- Binding: `@bind` and `@bind-Value`
- C# in attributes: `class="@cssClass"`, `disabled="@isDisabled"`
- DI and components: `@inject HttpClient Http`, `<MyComponent Title="@title" />`

Do not use `@` for plain HTML or static text.

`@bind` connects a value and its change event. For HTML inputs, the default pair is `value` + `onchange`, so `@bind` is enough. For components, you bind to the specific parameter name, such as `@bind-Value` (pairs `Value` + `ValueChanged`).

Other common value/event pairs:
- `<input type="text">` uses `value` + `onchange`
- `<input type="checkbox">` uses `checked` + `onchange`
- `<input type="range">` uses `value` + `oninput` (if you choose `@bind:event="oninput"`)

Examples:
```razor
<input @bind="playerName" />
<input type="checkbox" @bind="isReady" />
<input type="range" @bind="volume" @bind:event="oninput" />
<InputText @bind-Value="playerName" />
<InputNumber @bind-Value="boardSize" />
<InputDate @bind-Value="startDate" />
<InputSelect @bind-Value="difficulty">
    <option value="Easy">Easy</option>
    <option value="Hard">Hard</option>
</InputSelect>
```

---

## 6️⃣ Component Parameters and Events

Parameters let parents pass data to child components:

```razor
@code {
    [Parameter]
    public string Title { get; set; }
}
```

Child-to-parent communication uses `EventCallback`:

```razor
@code {
    [Parameter]
    public EventCallback<int> OnCellClicked { get; set; }
}
```

This is essential when you split the board into reusable pieces.

---

## 7️⃣ State and Re-rendering

- Components keep **state** (fields and properties in `@code`).
- When state changes, Blazor **re-renders** the component.
- This is the key difference from MVC page reloads.

### StateHasChanged (Manual Re-render)

- Blazor automatically re-renders after UI events like `@onclick`.
- Call `StateHasChanged()` when state changes **outside** a UI event, like timers, background tasks, or external callbacks.
- You rarely need it in normal button handlers because Blazor already refreshes the UI.

Example (timer-driven updates):

```razor
<p>Seconds: @seconds</p>

@code {
    private int seconds;
    private System.Timers.Timer? timer;

    protected override void OnInitialized()
    {
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += (_, __) =>
        {
            seconds++;
            InvokeAsync(StateHasChanged);
        };
        timer.Start();
    }
}
```

---

## 8️⃣ Conditional Rendering

Use `@if` and `@switch` to show or hide UI:

```razor
@if (gameOver)
{
    <p>Game Over</p>
}
else
{
    <p>Current player: @currentPlayer</p>
}
```

---

## 9️⃣ Forms and Validation (Intro)

For simple cases, plain `<input>` elements with `@bind` are sufficient — and that is what the solution uses:

```razor
<input class="form-control" @bind="player1Name" placeholder="Player 1" />
<button @onclick="StartGame">Start Game</button>
```

When you have multiple fields with validation rules, `EditForm` and data annotations offer a more structured approach:

```razor
<EditForm Model="model" OnValidSubmit="StartGame">
    <InputText @bind-Value="model.Player1Name" />
    <ValidationMessage For="() => model.Player1Name" />
    <button type="submit">Start</button>
</EditForm>
```

```csharp
public class StartGameModel
{
    [Required]
    public string Player1Name { get; set; }
}
```

---

## 1️⃣0️⃣ Component Lifecycle (Blazor Server)

Core lifecycle moments:
- `OnInitialized` / `OnInitializedAsync`: first-time setup (sync/async)
- `OnParametersSet` / `OnParametersSetAsync`: when parent parameters change
- `OnAfterRender` / `OnAfterRenderAsync`: runs after the UI renders (useful for JS interop)

Why it matters:
- Load data in `OnInitializedAsync`
- React to parent changes in `OnParametersSet`
- Avoid heavy work in rendering methods

---

## 1️⃣1️⃣ Dependency Injection in Components

Blazor uses the same DI container as ASP.NET Core:

```razor
@inject HttpClient Http
@inject TicTacToe.Core.GameEngine Engine
```

Use DI for services like API clients, state containers, and shared logic.

> **Note on lifetime:** In this solution, `GameEngine` is registered as `AddSingleton`, meaning all browser tabs share the same game instance. This is intentional for simplicity — it lets you focus on learning Blazor without worrying about per-user state. In lesson 8, SignalR will introduce a proper room-based model where each game is isolated.

---

## 1️⃣2️⃣ State Management Basics

Start simple:
- Local state in a component (`@code` fields)
- Parent passes state to child via parameters

When multiple components need the same state:
- Use a shared service registered in DI
- Optionally use `CascadingValue` for deep trees

---

## 1️⃣2️⃣a Mental Model (How Blazor “Thinks”)

- A component is **markup + C#** in one file.
- The UI is a **rendered result** of current component state.
- State changes → Blazor re-renders that component.
- Data flows **down** with parameters and **up** with `EventCallback`.
- If state changes outside UI events, call `StateHasChanged()`.

---

## 1️⃣2️⃣b Data Flow Map (TicTacToe UI)

- **Game state lives** in the page component (board, current player, status).
- **Board component** receives the board via `[Parameter]`.
- **Cell click** raises an `EventCallback<int>` back to the parent.
- **Parent updates state**, Blazor re-renders the board.

---

## 1️⃣2️⃣c Troubleshooting Checklist

- UI didn’t update? Confirm state actually changed.
- Event not firing? Check `@onclick` or `EventCallback` wiring.
- Binding not working? Verify `@bind` or `@bind-Value` matches the parameter name.
- Parameter not arriving? Ensure `[Parameter]` is set and you passed it in.
- Async updates? Use `InvokeAsync(StateHasChanged)` if needed.

---

## 1️⃣2️⃣d Debug Flow (UI Doesn’t Update)

Print this first:
```csharp
Console.WriteLine("Handler fired");
```

1. Did the **event fire**? Add a quick log in the handler.
2. Did you **change component state** (field/property in `@code`)?
3. Is the **state used in markup** (is it actually rendered)?
4. Is **data flow wired** (parameters down, `EventCallback` up)?
5. Is **binding correct** (`@bind` vs `@bind-Value`)?
6. Is the update **outside UI events**? Use `InvokeAsync(StateHasChanged)`.

Tiny example:

```razor
<button @onclick="AddMove">Add</button>
<p>@status</p>

@code {
    private string status = "Waiting";

    void AddMove()
    {
        Console.WriteLine("AddMove clicked");
        status = "Updated";
    }
}
```

---

## 1️⃣2️⃣e Common Pitfalls (Beginner Traps)

- Changing a local variable inside a loop and expecting the UI to update if it is not part of component state.

Example (wrong vs right):
```razor
@for (int i = 0; i < 3; i++)
{
    var temp = i * 2;
    <p>@temp</p>
}
```
```razor
@code {
    private int[] values = { 0, 2, 4 };
}

@for (int i = 0; i < values.Length; i++)
{
    <p>@values[i]</p>
}
```

- Mutating a list or array but not replacing it, then wondering why the UI does not refresh in some cases.

Example (mutate vs replace):
```razor
@code {
    private List<string> moves = new();

    void AddMove(string move)
    {
        moves.Add(move);
        // UI may not re-render in some async cases without StateHasChanged.
    }
}
```
```razor
@code {
    private List<string> moves = new();

    void AddMove(string move)
    {
        moves = moves.Append(move).ToList();
    }
}
```

- Forgetting `[Parameter]` on a child component property.

Example (missing vs correct):
```razor
@code {
    public string Title { get; set; } = "";
}
```
```razor
@code {
    [Parameter] public string Title { get; set; } = "";
}
```

- Using `@bind` with a component that expects `@bind-Value`.

Example (wrong vs right):
```razor
<InputText @bind="playerName" />
```
```razor
<InputText @bind-Value="playerName" />
```

- Calling async methods without `await`, then wondering why state updates are late or missing.

Example (wrong vs right):
```razor
@code {
    async Task LoadAsync()
    {
        LoadDataAsync();
        status = "Loaded";
    }
}
```
```razor
@code {
    async Task LoadAsync()
    {
        await LoadDataAsync();
        status = "Loaded";
    }
}
```

---

## 1️⃣2️⃣f Componentization Walkthrough (GameBoard → Cell)

Split the board into a reusable `GameBoard` and `Cell`:

`GameBoard.razor`
```razor
@using TicTacToe.Core

<div class="d-grid gap-2">
    @for (var i = 0; i < GameBoard.Size; i++)
    {
        <div class="d-flex justify-content-center">
            @for (var j = 0; j < GameBoard.Size; j++)
            {
                var pos = i * GameBoard.Size + j + 1;
                var cell = GameBoard.GetCell(i, j);

                <Cell Value="@cell"
                      Index="@pos"
                      Disabled="@(cell != '.' || !IsInProgress)"
                      OnClicked="OnCellClicked" />
            }
        </div>
    }
</div>

@code {
    [Parameter] public TicTacToe.Core.Board GameBoard { get; set; } = default!;
    [Parameter] public bool IsInProgress { get; set; }
    [Parameter] public EventCallback<int> OnCellClicked { get; set; }
}
```

`Cell.razor`
```razor
<button class="btn btn-outline-dark m-1"
        style="width: 60px; height: 60px; font-size: 24px"
        disabled="@Disabled"
        @onclick="() => OnClicked.InvokeAsync(Index)">
    @(Value == '.' ? "" : Value.ToString())
</button>

@code {
    [Parameter] public char Value { get; set; }
    [Parameter] public int Index { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback<int> OnClicked { get; set; }
}
```

In `Game.razor`, the board section becomes:

```razor
<GameBoard GameBoard="@Board"
           IsInProgress="@(Engine.Status == GameStatus.InProgress)"
           OnCellClicked="MakeMove" />
```

---

## 1️⃣2️⃣g Patterns to Try Next (Independent Practice)

- **Forms + validation**: `EditForm`, `InputText`, data annotations.
- **Shared state**: register a `GameStateService` in DI.
- **CascadingValue**: avoid passing the same parameter through many layers.
- **Loading states**: show “Loading…” during async calls.

---

## 1️⃣3️⃣ Build the Tic Tac Toe UI (Local State First)

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

Mini-exercise:
- Add a **current player** label and switch between X and O after each move.

Pair exercise:
- Extract the board into a **separate component** and pass the board state as a parameter.

---

## 1️⃣4️⃣ Optional Extension: Fetching Game State from the API

Only after the local UI works, you can connect to your Web API from week 5.

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

## 1️⃣5️⃣ Optional Extension: UI Error Handling

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

## 1️⃣6️⃣ Bonus: Styling the Board

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

## 1️⃣7️⃣ Wrap-up

In this lesson, you learned to:
- Translate MVC/Razor knowledge into Blazor components
- Build a **stateful UI** with C# event handling
- Re-render the UI by changing component state
- Optionally, connect to your Web API and handle errors

Next time: **SignalR & Multiplayer**! 🎮
