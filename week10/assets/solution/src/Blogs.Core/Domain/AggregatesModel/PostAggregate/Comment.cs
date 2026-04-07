namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class Comment : Entity< CommentId >
{
    private string _content = null!;

    public string Content
    {
        get => _content;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PostException( PostExceptionCode.CommentTextNullOrWhiteSpace );

            if ( value.Length > 1024 )
                throw new PostException( PostExceptionCode.CommentTextTooLong );

            _content = value;
        }
    }

    public PersonId AuthorId { get; set; } = null!;
    public PostId PostId { get; set; } = null!;
    public virtual Post Post { get; } = null!;

    protected Comment( string content ) => Content = content;

    public Comment( Post post, PersonId authorId, string content )
        : this( content )
    {
        Post = post;
        PostId = post.Id;
        AuthorId = authorId;
        Content = content;
        AddDomainEvent( new CommentAdded( PostId ) );
    }
}
