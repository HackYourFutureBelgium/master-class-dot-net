namespace Blogs.Infrastructure.Persistence.Repositories;

[ ExcludeFromCodeCoverage ]
public class PostRepository( ILogger< PostRepository > logger, Context context ) : IPostRepository
{
    private readonly ILogger< PostRepository > _logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly Context _context = context ?? throw new ArgumentNullException( nameof( context ) );

    public async Task< Post > GetByIdAsync( PostId postId, CancellationToken cancellationToken = default )
    {
        var post = await _context.Posts.SingleOrDefaultAsync( p => p.Id == postId, cancellationToken );

        if ( post is null )
            throw new EntityNotFoundException< Post >( nameof( Post.Id ), postId );

        return post;
    }
}
