namespace Blogs.Core.Domain.AggregatesModel.BlogAggregate;

public class BlogExceptionCode : Enumeration
{
    public static readonly BlogExceptionCode TitleNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000001" ), "TITLE_NULL_OR_WHITE_SPACE" );
    public static readonly BlogExceptionCode TitleTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000002" ), "TITLE_TOO_LONG" );
    public static readonly BlogExceptionCode DescriptionTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000003" ), "DESCRIPTION_TOO_LONG" );

    private BlogExceptionCode( Guid id, string name )
        : base( id, name ) { }
}
