namespace Blogs.Infrastructure.Persistence;

[ ExcludeFromCodeCoverage ]
public class DesignTimeContextFactory : DesignTimeDatabaseContextFactory< Context >
{
    protected override Context InitializeContext( DbContextOptions< Context > options )
        => new( options );
}
