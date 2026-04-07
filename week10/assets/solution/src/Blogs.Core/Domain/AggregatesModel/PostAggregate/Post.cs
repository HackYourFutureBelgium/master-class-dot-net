namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class Post : Entity< PostId >, IAggregateRoot
{
    private PostStatus _postStatus = null!;
    private string _title = null!;
    private string _content = null!;
    private DateTime? _publishedOn;
    private DateTime? _archivedOn;
    private readonly List< Comment > _comments = [ ];
    private readonly List< Tag > _tags = [ ];

    public string Title
    {
        get => _title;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PostException( PostExceptionCode.TitleNullOrWhiteSpace );

            if ( value.Length > 256 )
                throw new PostException( PostExceptionCode.TitleTooLong );

            _title = value;
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PostException( PostExceptionCode.ContentNullOrWhiteSpace );

            if ( value.Length > 4096 )
                throw new PostException( PostExceptionCode.ContentTooLong );

            _content = value;
        }
    }

    public DateTime? PublishedOn => _publishedOn;
    public DateTime? ArchivedOn => _archivedOn;
    public BlogId BlogId { get; private set; } = null!;
    public Guid PostStatusId { get; private set; }

    public virtual PostStatus PostStatus
    {
        get => Enumeration.FromValue< PostStatus >( PostStatusId );
        private set
        {
            _postStatus = null!;
            PostStatusId = value.Id;
        }
    }

    public virtual IReadOnlyCollection< Comment > Comments => _comments.AsReadOnly();
    public virtual IReadOnlyCollection< Tag > Tags => _tags.AsReadOnly();

    public Post( BlogId blogId, string title, string content )
    {
        PostStatus = PostStatus.Draft;
        BlogId = blogId;
        Title = title;
        Content = content;
    }

    public void Publish()
    {
        if ( _publishedOn is not null )
            throw new PostException( PostExceptionCode.AlreadyPublished );

        _publishedOn = DateTime.UtcNow;
        PostStatus = PostStatus.Published;
        AddDomainEvent( new PostPublished( Id ) );
    }

    public void Unpublish()
    {
        if ( _publishedOn is null )
            throw new PostException( PostExceptionCode.NotPublished );

        _publishedOn = null;
        PostStatus = PostStatus.Draft;
    }

    public void Archive()
    {
        if ( _archivedOn is not null )
            throw new PostException( PostExceptionCode.AlreadyPublished );

        _archivedOn = DateTime.UtcNow;
        PostStatus = PostStatus.Archived;
    }

    public Comment AddComment( PersonId authorId, string content )
    {
        var comment = new Comment( this, authorId, content );
        _comments.Add( comment );
        return comment;
    }

    public void AddComment( Comment comment )
    {
        _comments.Add( comment );
    }

    public void DeleteComment( Comment comment )
    {
        _comments.Remove( comment );
    }

    public Tag AddTag( string value )
    {
        var tag = new Tag( value );
        _tags.Add( tag );
        return tag;
    }

    public void AddTag( Tag tag )
    {
        if ( !_tags.Contains( tag ) )
            _tags.Add( tag );
    }

    public void DeleteTag( Tag tag )
    {
        if ( _tags.Contains( tag ) )
            _tags.Remove( tag );
    }
}
