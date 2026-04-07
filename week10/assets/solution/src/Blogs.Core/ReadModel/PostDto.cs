namespace Blogs.Core.ReadModel;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? PublishedOn { get; set; }
    public DateTime? ArchivedOn { get; set; }
    public PostStatusDto PostStatus { get; set; }
    public Guid BlogId { get; set; }
    public IEnumerable< string > Tags { get; set; } = new List< string >();
    public IEnumerable< CommentDto > Comments { get; set; } = new List< CommentDto >();

    public static explicit operator PostDto( Post post )
    {
        PostStatusDto postStatus;

        if ( post.PostStatus == Domain.AggregatesModel.PostAggregate.PostStatus.Draft )
            postStatus = PostStatusDto.Draft;
        else if ( post.PostStatus == Domain.AggregatesModel.PostAggregate.PostStatus.Published )
            postStatus = PostStatusDto.Published;
        else if ( post.PostStatus == Domain.AggregatesModel.PostAggregate.PostStatus.Archived )
            postStatus = PostStatusDto.Archived;
        else
            throw new ArgumentOutOfRangeException( nameof( post.PostStatus ) );

        return new PostDto
        {
            Id = post.Id.Value,
            Title = post.Title,
            Content = post.Content,
            PublishedOn = post.PublishedOn,
            ArchivedOn = post.ArchivedOn,
            PostStatus = postStatus,
            BlogId = post.BlogId.Value,
            Tags = post.Tags.Select( t => t.Value ).ToList(),
            Comments = post.Comments.Select( c => ( CommentDto )c ).ToList()
        };
    }
}
