namespace Blogs.Infrastructure.Persistence.Queries;

[ ExcludeFromCodeCoverage ]
public class PostQueries( ILogger< PostQueries > logger, IConnectionFactory connectionFactory ) : IPostQueries
{
    private readonly ILogger< PostQueries > _logger = logger
                                                   ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IConnectionFactory _connectionFactory = connectionFactory
                                                          ?? throw new ArgumentNullException( nameof( connectionFactory ) );

    public Task< (IEnumerable< PostDto >, uint) > FindPostsAsync(
        string? searchTerm,
        IEnumerable< string >? tags,
        uint pageIndex = 1,
        uint pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public async Task< PostDto > GetPostAsync( PostId postId, CancellationToken cancellationToken = default )
    {
        var query = """
                    SELECT p.Id
                          ,p.Title
                          ,p.Content
                          ,p.PublishedOn
                          ,p.ArchivedOn
                          ,PostStatus = ps.Name
                          ,p.BlogId
                          ,p.CreatedOn
                          ,p.LastModifiedOn
                      FROM Post p
                      LEFT JOIN PostStatus ps
                        ON p.PostStatusId = ps.Id
                     WHERE p.Id = @PostId;
                    SELECT t.Value
                      FROM Tag t
                     WHERE t.PostId = @PostId;
                    SELECT c.Id
                          ,c.Content
                          ,c.AuthorId
                          ,c.CreatedOn
                          ,c.LastModifiedOn
                      FROM Comment c
                     WHERE c.PostId = @PostId
                    """;
        await using var connection = _connectionFactory.CreateConnection();
        await using var multi = await connection.QueryMultipleAsync(
            query,
            new { PostId = postId.Value }
        );
        var postDto = await multi.ReadSingleOrDefaultAsync< PostDto >();

        if ( postDto is null )
            throw new EntityNotFoundException< Post >( nameof( Post.Id ), postId );

        var tags = await multi.ReadAsync< string >();
        postDto.Tags = tags;
        var comments = await multi.ReadAsync< CommentDto >();
        postDto.Comments = comments;
        return postDto;
    }
}
