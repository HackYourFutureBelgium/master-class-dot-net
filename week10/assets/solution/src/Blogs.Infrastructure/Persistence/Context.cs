namespace Blogs.Infrastructure.Persistence;

[ ExcludeFromCodeCoverage ]
public sealed class Context( DbContextOptions options ) : DatabaseContextWithOutbox( options )
{
    public DbSet< Person > People { get; init; } = null!;
    public DbSet< Blog > Blogs { get; init; } = null!;
    public DbSet< Post > Posts { get; init; } = null!;

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        modelBuilder.ApplyConfigurationsFromAssembly( Assembly.GetExecutingAssembly() );
        base.OnModelCreating( modelBuilder );
    }
}
