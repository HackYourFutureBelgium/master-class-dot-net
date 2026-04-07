namespace Blogs.Core.Domain.AggregatesModel.BlogAggregate;

public class Blog : Entity< BlogId >, IAggregateRoot
{
    private string _title = null!;
    private string? _description;

    public string Title
    {
        get => _title;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new BlogException( BlogExceptionCode.TitleNullOrWhiteSpace );

            if ( value.Length > 256 )
                throw new BlogException( BlogExceptionCode.TitleTooLong );

            _title = value;
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if ( value?.Length > 1024 )
                throw new BlogException( BlogExceptionCode.DescriptionTooLong );

            _description = value;
        }
    }

    public PersonId OwnerId { get; } = null!;

    protected Blog( string title, string? description = null )
    {
        Title = title;
        Description = description;
    }

    public Blog( string title, PersonId ownerId, string? description = null )
        : this( title, description )
        => OwnerId = ownerId;
}
