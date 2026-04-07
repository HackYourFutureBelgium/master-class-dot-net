namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class PostExceptionCode : Enumeration
{
    public static readonly PostExceptionCode TitleNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000001" ), "TITLE_NULL_OR_WHITE_SPACE" );
    public static readonly PostExceptionCode TitleTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000002" ), "TITLE_TOO_LONG" );
    public static readonly PostExceptionCode ContentNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000003" ), "CONTENT_NULL_OR_WHITE_SPACE" );
    public static readonly PostExceptionCode ContentTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000004" ), "CONTENT_TOO_LONG" );
    public static readonly PostExceptionCode AlreadyPublished = new( Guid.Parse( "00000000-0000-0000-0000-000000000006" ), "ALREADY_PUBLISHED" );
    public static readonly PostExceptionCode NotPublished = new( Guid.Parse( "00000000-0000-0000-0000-000000000007" ), "NOT_PUBLISHED" );
    public static readonly PostExceptionCode AlreadyArchived = new( Guid.Parse( "00000000-0000-0000-0000-000000000008" ), "ALREADY_ARCHIVED" );

    public static readonly PostExceptionCode CommentTextNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000101" ), "COMMENT_TEXT_NULL_OR_WHITE_SPACE" );
    public static readonly PostExceptionCode CommentTextTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000102" ), "COMMENT_TEXT_TOO_LONG" );

    public static readonly PostExceptionCode TagNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000201" ), "TAG_NULL_OR_WHITE_SPACE" );
    public static readonly PostExceptionCode TagTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000202" ), "TAG_TOO_LONG" );
    public static readonly PostExceptionCode NewErrorWithoutName = new( Guid.Parse( "00000000-0000-0000-0000-000000000203" ), "NEW_ERROR_WITHOUT_NAME" );

    private PostExceptionCode( Guid id, string name )
        : base( id, name ) { }
}
