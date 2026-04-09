using RootBlocks.Aggregate.Abstractions;
using RootBlocks.Persistence.Abstractions;

namespace Blogs.Application.Tests.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public List< IAggregateRoot > NewEntities    { get; } = [ ];
    public List< IAggregateRoot > DirtyEntities  { get; } = [ ];
    public List< IAggregateRoot > DeletedEntities { get; } = [ ];
    public bool CommitCalled { get; private set; }

    public void RegisterNew( IAggregateRoot aggregateRoot )     => NewEntities.Add( aggregateRoot );
    public void RegisterClean( IAggregateRoot aggregateRoot )   { }
    public void RegisterDirty( IAggregateRoot aggregateRoot )   => DirtyEntities.Add( aggregateRoot );
    public void RegisterDeleted( IAggregateRoot aggregateRoot ) => DeletedEntities.Add( aggregateRoot );

    public void   Commit()                                           { CommitCalled = true; }
    public Task   CommitAsync( CancellationToken ct = default )      { CommitCalled = true; return Task.CompletedTask; }
    public void   Rollback()                                         { }
    public void   BeginTransaction()                                 { }
    public Task   BeginTransactionAsync()                            => Task.CompletedTask;
    public void   CommitTransaction()                                { }
    public Task   CommitTransactionAsync()                           => Task.CompletedTask;
    public void   RollbackTransaction()                              { }
}
