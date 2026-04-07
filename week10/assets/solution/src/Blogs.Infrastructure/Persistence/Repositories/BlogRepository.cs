namespace Blogs.Infrastructure.Persistence.Repositories;

[ ExcludeFromCodeCoverage ]
public class BlogRepository( ILogger< BlogRepository > logger, Context context ) : IBlogRepository
{
    private readonly ILogger< BlogRepository > _logger = logger
                                                      ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly Context _context = context
                                     ?? throw new ArgumentNullException( nameof( context ) );

    public async Task< Blog > GetByIdAsync( BlogId blogId, CancellationToken cancellationToken = default )
    {
        var blog = await _context.Blogs.SingleOrDefaultAsync( b => b.Id == blogId, cancellationToken );

        if ( blog is null )
            throw new EntityNotFoundException< Blog >( nameof( Blog.Id ), blogId );

        return blog;
    }
}
