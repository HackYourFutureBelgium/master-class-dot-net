namespace Blogs.Infrastructure.Persistence.Queries;

[ ExcludeFromCodeCoverage ]
public class BlogQueries( ILogger< BlogQueries > logger, IConnectionFactory connectionFactory ) : IBlogQueries
{
    private readonly ILogger< BlogQueries > _logger = logger
                                                   ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IConnectionFactory _connectionFactory = connectionFactory
                                                          ?? throw new ArgumentNullException( nameof( connectionFactory ) );

    public async Task< BlogDto > GetBlogAsync( BlogId blogId, CancellationToken cancellationToken = default )
    {
        var query = """
                    SELECT b.Id
                          ,b.Title
                          ,b.Description
                          ,b.OwnerId
                          ,b.CreatedOn
                          ,b.LastModifiedOn
                      FROM Blog b
                     WHERE b.Id = @BlogId
                    """;
        await using var connection = _connectionFactory.CreateConnection();
        var parameters = new { BlogId = blogId.Value };
        var blogDto = await connection.QuerySingleOrDefaultAsync< BlogDto >( query, parameters );

        if ( blogDto is null )
            throw new EntityNotFoundException< Blog >( nameof( Blog.Id ), blogId );

        return blogDto;
    }
}
