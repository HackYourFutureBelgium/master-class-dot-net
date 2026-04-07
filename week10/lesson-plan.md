# Lesson 10: Production-Grade Architecture — A Blogs API Walkthrough

## Overview

This lesson is a demonstration, not a hands-on exercise. We walk through a fully working, production-grade **Blogs REST API** built with .NET 9. The goal is to show how all the concepts from the previous lessons combine into a real-world architecture, and to name every pattern present so you know what to look for — and look up — when you encounter them in the wild.

The solution lives in `week10/assets/solution/`. Open it, follow along, and ask questions.

---

## Solution Structure

```
Blogs.sln
└── src/
    ├── Blogs.Core           # Domain model + read model interfaces
    ├── Blogs.Application    # Use cases (commands + handlers)
    ├── Blogs.Infrastructure # Persistence (EF Core + Dapper)
    └── Blogs.Api            # HTTP layer (controllers + Swagger)
```

Dependencies flow strictly **inward**:

```
Api → Application → Core
Api → Infrastructure → Core
```

`Core` has zero external dependencies. Everything else depends on it, never the other way around.

---

## Pattern 1 — Clean Architecture (Onion Architecture)

**What it is:** Organises code into concentric layers. The innermost layer (Domain) knows nothing about databases, HTTP, or frameworks. Outer layers implement the interfaces defined by inner layers.

**Why it matters:** You can swap SQL Server for PostgreSQL, or REST for gRPC, without touching domain logic.

```
┌──────────────────────────────┐
│  Blogs.Api (HTTP)            │
│  ┌────────────────────────┐  │
│  │  Blogs.Application     │  │
│  │  ┌──────────────────┐  │  │
│  │  │  Blogs.Core      │  │  │
│  │  │  (Domain)        │  │  │
│  │  └──────────────────┘  │  │
│  └────────────────────────┘  │
│  Blogs.Infrastructure        │
└──────────────────────────────┘
```

**Where to look:** The `.csproj` references tell the whole story — `Blogs.Core.csproj` references nothing; `Blogs.Infrastructure.csproj` references `Blogs.Core`.

---

## Pattern 2 — Domain-Driven Design (DDD)

DDD is not a single pattern — it is a collection of concepts for modelling complex business domains. Several of them appear here.

### 2a. Aggregate & Aggregate Root

An **aggregate** is a cluster of related objects treated as a single unit for persistence and consistency. The **aggregate root** is the only public entry point into the cluster.

```
Post (aggregate root)
├── Comment (entity, owned by Post)
└── Tag    (value object, owned by Post)
```

The `Post` class exposes `_comments` and `_tags` as private `List<T>` fields, publicly surfaced only as `IReadOnlyCollection<T>`. Nothing outside the aggregate can add or remove comments directly:

```csharp
// Post.cs
private readonly List<Comment> _comments = [];
private readonly List<Tag>     _tags     = [];

public virtual IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
public virtual IReadOnlyCollection<Tag>     Tags     => _tags.AsReadOnly();

public Comment AddComment(PersonId authorId, string content)
{
    var comment = new Comment(this, authorId, content);
    _comments.Add(comment);
    return comment;
}
```

### 2b. Entity vs Value Object

| Concept | Identity | Mutability | Example here |
|---|---|---|---|
| **Entity** | Has a unique ID | Can change over time | `Post`, `Comment`, `Blog`, `Person` |
| **Value Object** | Identified by its value | Immutable | `Tag` |

`Tag` has no ID column. Two tags with the same value are considered equal:

```csharp
// Tag.cs
public class Tag : ValueObject
{
    private readonly string _value = null!;

    public string Value
    {
        get => _value;
        private init { /* validation */ _value = value; }
    }

    public Tag(string value) => Value = value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;   // equality is based on this
    }
}
```

### 2c. Domain Events

A **domain event** is something meaningful that happened in the domain. Aggregates raise them; other parts of the system can react without the aggregate knowing anything about them.

Three domain events exist:

| Event | Raised when |
|---|---|
| `PersonRegistered` | A new `Person` is created |
| `PostPublished` | A `Post` transitions to Published state |
| `CommentAdded` | A comment is added to a post |

```csharp
// Person.cs — raises an event in the constructor
public Person(string fullName, string emailAddress)
{
    FullName     = fullName;
    EmailAddress = emailAddress;
    AddDomainEvent(new PersonRegistered(Id, FullName, EmailAddress));
}

// Post.cs — raises an event when business action completes
public void Publish()
{
    if (_publishedOn is not null)
        throw new PostException(PostExceptionCode.AlreadyPublished);

    _publishedOn = DateTime.UtcNow;
    PostStatus   = PostStatus.Published;
    AddDomainEvent(new PostPublished(Id));
}
```

The `UnitOfWork` dispatches collected events after the database transaction commits. This is the **Outbox Pattern** (see Pattern 11).

### 2d. Ubiquitous Language

Method names mirror the business vocabulary. You don't `SetStatus("Published")` — you call `Publish()`. You don't `SetStatus("Draft")` — you call `Unpublish()`. The code reads like the business does.

---

## Pattern 3 — Strongly-Typed IDs

Passing a raw `Guid` everywhere is error-prone. You can accidentally pass a `PersonId` where a `BlogId` is expected — the compiler won't complain.

Strongly-typed IDs make this a **compile-time error**:

```csharp
// BlogId.cs
[JsonConverter(typeof(IdentityJsonConverter<BlogId>))]
[TypeConverter(typeof(IdentityTypeConverter<BlogId>))]
public class BlogId : Identity;

// PersonId.cs
public class PersonId : Identity;
```

Now `IBlogRepository.GetByIdAsync(BlogId)` cannot accidentally accept a `PersonId`. The `JsonConverter` and `TypeConverter` attributes handle serialisation and model binding automatically.

---

## Pattern 4 — Enumeration Pattern (Rich Enums)

Standard C# `enum` values are just integers. They cannot carry behaviour, and their names are strings that vanish after compilation. The **Enumeration pattern** replaces raw enums with classes that have stable IDs (Guids here) and names.

```csharp
// PostStatus.cs
public class PostStatus : Enumeration
{
    public static readonly PostStatus Draft     = new(Guid.Parse("00000000-0000-0000-0000-000000000001"), nameof(Draft));
    public static readonly PostStatus Published = new(Guid.Parse("00000000-0000-0000-0000-000000000002"), nameof(Published));
    public static readonly PostStatus Archived  = new(Guid.Parse("00000000-0000-0000-0000-000000000003"), nameof(Archived));

    public PostStatus(Guid id, string name) : base(id, name) { }
}
```

The same pattern applies to error codes:

```csharp
// BlogExceptionCode.cs
public class BlogExceptionCode : Enumeration
{
    public static readonly BlogExceptionCode TitleNullOrWhiteSpace = new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "TITLE_NULL_OR_WHITE_SPACE");
    public static readonly BlogExceptionCode TitleTooLong          = new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "TITLE_TOO_LONG");
    public static readonly BlogExceptionCode DescriptionTooLong    = new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "DESCRIPTION_TOO_LONG");
}
```

Because they have stable Guids, these values survive refactoring (renaming `Draft` to `Pending` doesn't break old data in the database).

---

## Pattern 5 — Domain Validation (Fail Fast)

Validation lives inside the domain entities, not in controllers or validators. Property setters throw typed exceptions immediately if the invariant is violated:

```csharp
// Post.cs
public string Title
{
    get => _title;
    set
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PostException(PostExceptionCode.TitleNullOrWhiteSpace);

        if (value.Length > 256)
            throw new PostException(PostExceptionCode.TitleTooLong);

        _title = value;
    }
}
```

The domain object is always in a valid state. You can never construct a `Post` with an empty title.

---

## Pattern 6 — Repository Pattern

A **repository** provides a collection-like interface for accessing aggregates. The domain defines the interface; infrastructure implements it.

```csharp
// Core (domain interface)
public interface IPostRepository
{
    Task<Post> GetByIdAsync(PostId postId, CancellationToken cancellationToken = default);
}

// Infrastructure (EF Core implementation)
public class PostRepository(ILogger<PostRepository> logger, Context context) : IPostRepository
{
    public async Task<Post> GetByIdAsync(PostId postId, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.SingleOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (post is null)
            throw new EntityNotFoundException<Post>(nameof(Post.Id), postId);

        return post;
    }
}
```

The `EntityNotFoundException` is a well-known exception that the HTTP layer can map to a 404.

---

## Pattern 7 — Unit of Work

The **Unit of Work** tracks aggregate changes and commits them as a single atomic transaction. It also dispatches domain events after the commit.

```csharp
// UnitOfWork.cs (delegates to RootBlocks base)
public class UnitOfWork(Context context, IEventPublisher eventPublisher)
    : RootBlocks.Persistence.EntityFramework.UnitOfWork(context, eventPublisher);
```

Usage in a command handler:

```csharp
var post = new Post(request.BlogId, request.Title, request.Content);
_unitOfWork.RegisterNew(post);      // marks as "to insert"
await _unitOfWork.CommitAsync();    // persists + dispatches domain events
```

The three tracking methods are:
- `RegisterNew(aggregate)` — INSERT
- `RegisterDirty(aggregate)` — UPDATE
- `RegisterDeleted(aggregate)` — DELETE

---

## Pattern 8 — CQRS (Command Query Responsibility Segregation)

**Commands** change state. **Queries** read state. They use different models and different data access paths.

```
┌──────────────┐     Commands (write)     ┌────────────────────┐
│   Controller │ ──────────────────────► │  EF Core + UnitOfWork │
│              │                          └────────────────────┘
│              │     Queries (read)       ┌────────────────────┐
│              │ ──────────────────────► │  Dapper + raw SQL   │
└──────────────┘                          └────────────────────┘
```

### Commands use MediatR as a command dispatcher:

MediatR is used here not as the classic Mediator design pattern, but as a **command dispatcher** — a mechanism to route each command or query record to its dedicated handler without the controller needing to reference the handler directly.

```csharp
// Command — immutable record, carries input
public record PublishPostCommand(PostId PostId) : IRequest;

// Command Handler — one handler per command
public class PublishPostCommandHandler(...) : IRequestHandler<PublishPostCommand>
{
    public async Task Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        post.Publish();
        _unitOfWork.RegisterDirty(post);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
```

Controllers dispatch commands via `_mediator.Send(...)` and know nothing about the handlers:

```csharp
// Controller
await _mediator.Send(new PublishPostCommand(id));
```

Registration — MediatR scans all assemblies and wires up handlers automatically:

```csharp
// Application/Extensions/ServiceCollectionExtensions.cs
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
```

### Queries use hand-written SQL via Dapper:

The read model returns flat DTOs optimised for display — no ORM overhead, no lazy loading, no tracking. See Pattern 11 for the full query example.

---

## Pattern 9 — Outbox Pattern (Domain Event Dispatch)

Domain events raised inside aggregates are not dispatched immediately. They are collected during the transaction and published **after** the database commit succeeds. This guarantees that events are only fired for changes that actually persisted.

The `DatabaseContextWithOutbox` base class (from the `RootBlocks` library) stores events in a transactional outbox. The `UnitOfWork` flushes them after `CommitAsync()`.

```
Post.Publish()
  └─ AddDomainEvent(new PostPublished(Id))     ← collected, not fired yet

UnitOfWork.CommitAsync()
  ├─ SaveChanges()                             ← database row written
  └─ IEventPublisher.Publish(PostPublished)    ← event dispatched
```

---

## Pattern 10 — Soft Delete

`Person` entities are never physically deleted. Instead, a `DeletedOn` timestamp is set. All queries filter `WHERE p.DeletedOn IS NULL`.

```csharp
// PersonConfiguration.cs
public class PersonConfiguration : SoftDeletableEntityConfiguration<Person, PersonId>
```

```sql
-- PersonQueries.cs
SELECT p.Id, p.FullName, ...
  FROM Person p
 WHERE p.DeletedOn IS NULL      -- soft-delete filter
   AND p.Id = @PersonId
```

This preserves referential integrity (a `Blog` still has its `OwnerId`) and enables audit trails.

---

## Pattern 11 — Read Model with Dapper (Query Side of CQRS)

For reads, the application bypasses EF Core entirely and uses **Dapper** — a lightweight micro-ORM that maps raw SQL results to DTOs.

### Multi-result query (one round-trip, three result sets):

```csharp
// PostQueries.cs
var query = """
    SELECT p.Id, p.Title, p.Content, p.PublishedOn, p.ArchivedOn,
           PostStatus = ps.Name, p.BlogId, p.CreatedOn, p.LastModifiedOn
      FROM Post p
      LEFT JOIN PostStatus ps ON p.PostStatusId = ps.Id
     WHERE p.Id = @PostId;

    SELECT t.Value
      FROM Tag t
     WHERE t.PostId = @PostId;

    SELECT c.Id, c.Content, c.AuthorId, c.CreatedOn, c.LastModifiedOn
      FROM Comment c
     WHERE c.PostId = @PostId
    """;

await using var multi = await connection.QueryMultipleAsync(query, new { PostId = postId.Value });
var postDto    = await multi.ReadSingleOrDefaultAsync<PostDto>();
var tags       = await multi.ReadAsync<string>();
var comments   = await multi.ReadAsync<CommentDto>();
postDto.Tags     = tags;
postDto.Comments = comments;
```

### Paginated search with dynamic ORDER BY:

```csharp
// PersonQueries.cs — case-insensitive search, server-side sorting, pagination
ORDER BY
  CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'fullName' THEN FullName END,
  CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'fullName' THEN FullName END DESC,
  ...
OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
```

---

## Pattern 12 — Explicit Operator Conversion (Domain → DTO)

Rather than a separate mapper class, DTOs define an `explicit operator` that converts from the domain entity. Mapping logic is co-located with the DTO:

```csharp
// PostDto.cs
public static explicit operator PostDto(Post post)
{
    var postStatus = post.PostStatus == PostStatus.Draft      ? PostStatusDto.Draft
                   : post.PostStatus == PostStatus.Published  ? PostStatusDto.Published
                   : post.PostStatus == PostStatus.Archived   ? PostStatusDto.Archived
                   : throw new ArgumentOutOfRangeException(nameof(post.PostStatus));

    return new PostDto
    {
        Id          = post.Id.Value,
        Title       = post.Title,
        Content     = post.Content,
        PublishedOn = post.PublishedOn,
        PostStatus  = postStatus,
        Tags        = post.Tags.Select(t => t.Value).ToList(),
        Comments    = post.Comments.Select(c => (CommentDto)c).ToList()
    };
}
```

Usage in a command handler:

```csharp
var postDto = (PostDto)post;   // clean, explicit, IDE-navigable
```

---

## Pattern 13 — EF Core Fluent Configuration

All entity-to-table mappings live in dedicated `IEntityTypeConfiguration<T>` classes, not in the `DbContext` or entity classes. `OnModelCreating` scans the assembly for all of them automatically:

```csharp
// Context.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    base.OnModelCreating(modelBuilder);
}
```

```csharp
// PostConfiguration.cs
public class PostConfiguration : EntityConfiguration<Post, PostId>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Title).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Content).HasMaxLength(4096).IsRequired();

        // Private backing fields for collections
        builder.Metadata.FindNavigation(nameof(Post.Comments))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Post.Tags))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Blog>().WithMany()
               .HasForeignKey(p => p.BlogId).OnDelete(DeleteBehavior.Cascade);
    }
}
```

The `SetPropertyAccessMode(PropertyAccessMode.Field)` call tells EF Core to use the private `_comments` / `_tags` fields when loading navigation properties — this is what allows the collections to remain encapsulated.

---

## Pattern 14 — JSON Patch (Partial Updates)

Instead of `PUT` (replace the whole resource), PATCH endpoints accept a **JSON Patch document** — a list of operations (add, remove, replace, copy, move, test) applied to the existing resource.

```http
PATCH /blog/3fa85f64...
Content-Type: application/json-patch+json

[
  { "op": "replace", "path": "/title", "value": "New Title" }
]
```

```csharp
// UpdateBlogCommand.cs
public record UpdateBlogCommand(BlogId BlogId, JsonPatchDocument<Blog> BlogPatchDocument) : IRequest;

// UpdateBlogCommandHandler.cs
var blog = await _blogRepository.GetByIdAsync(request.BlogId, cancellationToken);
request.BlogPatchDocument.ApplyTo(blog);    // applies the patch operations
_unitOfWork.RegisterDirty(blog);
await _unitOfWork.CommitAsync(cancellationToken);
```

---

## Pattern 15 — Composition Root & Service Registration Extensions

All dependency injection wiring happens in one place (the entry point), but each layer owns its own `ServiceCollectionExtensions` so the API layer doesn't need to know the internals of each layer:

```csharp
// Program.cs
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// Application/Extensions/ServiceCollectionExtensions.cs
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(...));

// Infrastructure/Extensions/ServiceCollectionExtensions.cs
services.AddDbContext<Context>(o => o.UseLazyLoadingProxies().UseSqlServer(connectionString));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IPostRepository, PostRepository>();
services.AddScoped<IPersonQueries, PersonQueries>();
// ...
```

---

## Pattern 16 — Structured Logging with Serilog

The application uses **Serilog** with structured, queryable log events rather than plain text strings. Configuration comes from `appsettings.json`, with a bootstrap logger for startup errors:

```csharp
// Program.cs — bootstrap logger (before DI is ready)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// Replaced by full config-driven logger once the host is built
builder.Host.UseSerilog((context, _, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
```

---

## Pattern 17 — Health Checks

A `/health` endpoint provides a machine-readable liveness/readiness signal, used by Kubernetes, load balancers, and deployment pipelines:

```csharp
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

---

## Pattern 18 — Response Compression

HTTP responses are compressed with **Brotli** (superior to gzip for text/JSON) before being sent to the client:

```csharp
builder.Services.AddResponseCompression(o => o.Providers.Add<BrotliCompressionProvider>());
app.UseResponseCompression();
```

---

## Pattern 19 — SQL Injection Prevention (EncodeForSqlLike)

`LIKE` queries are vulnerable if user input contains `%` or `_`. A utility extension sanitises the search term before embedding it:

```csharp
// StringExtensions.cs
public static string EncodeForSqlLike(this string value) =>
    value.Replace("%", "[%]").Replace("_", "[_]");

// PersonQueries.cs — usage
LikeSearchTerm = !string.IsNullOrWhiteSpace(searchTerm)
    ? $"%{searchTerm.EncodeForSqlLike()}%"
    : string.Empty,
```

All query parameters are passed as Dapper parameters (never string-concatenated into SQL), which prevents SQL injection by design.

---

## Pattern 20 — Constructor Guard Clauses

Every class guards its constructor parameters against null, producing a clear exception with the parameter name rather than a cryptic `NullReferenceException` later:

```csharp
public class PublishPostCommandHandler(
    ILogger<PublishPostCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler<PublishPostCommand>
{
    private readonly ILogger<PublishPostCommandHandler> _logger =
        logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUnitOfWork _unitOfWork =
        unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IPostRepository _postRepository =
        postRepository ?? throw new ArgumentNullException(nameof(postRepository));
}
```

---

## What Is Left Intentionally Incomplete

Two things are present but not fully implemented — they are left as exercises or conversation starters:

1. **`FindPostsAsync`** in `PostQueries.cs` throws `NotImplementedException`. The pagination + filtering SQL is left for you to write, following the `PersonQueries.FindPeopleAsync` implementation as a model.

2. **Global Exception Handling** and **Correlation Headers** are commented out in `Program.cs`. In a production system these would map domain exceptions (like `EntityNotFoundException`) to the correct HTTP status codes automatically, and attach a correlation ID to every request/response for distributed tracing.

---

## Summary — Pattern Inventory

| # | Pattern | Layer |
|---|---|---|
| 1 | Clean Architecture / Onion | All |
| 2a | DDD — Aggregate & Aggregate Root | Core |
| 2b | DDD — Entity vs Value Object | Core |
| 2c | DDD — Domain Events | Core |
| 2d | DDD — Ubiquitous Language | Core |
| 3 | Strongly-Typed IDs | Core |
| 4 | Enumeration Pattern (Rich Enums) | Core |
| 5 | Domain Validation (Fail Fast) | Core |
| 6 | Repository Pattern | Core / Infrastructure |
| 7 | Unit of Work | Infrastructure |
| 8 | CQRS (with MediatR as command dispatcher) | Application / Infrastructure |
| 9 | Outbox Pattern | Infrastructure |
| 10 | Soft Delete | Infrastructure |
| 11 | Read Model with Dapper | Infrastructure |
| 12 | Explicit Operator Conversion | Core |
| 13 | EF Core Fluent Configuration | Infrastructure |
| 14 | JSON Patch (Partial Updates) | Application / API |
| 15 | Composition Root & DI Extensions | API |
| 16 | Structured Logging (Serilog) | API |
| 17 | Health Checks | API |
| 18 | Response Compression (Brotli) | API |
| 19 | SQL Injection Prevention | Infrastructure |
| 20 | Constructor Guard Clauses | All |
