namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class PostStatus : Enumeration
{
    public static readonly PostStatus Draft = new PostStatus( Guid.Parse( "00000000-0000-0000-0000-000000000001" ), nameof( Draft ) );
    public static readonly PostStatus Published = new PostStatus( Guid.Parse( "00000000-0000-0000-0000-000000000002" ), nameof( Published ) );
    public static readonly PostStatus Archived = new PostStatus( Guid.Parse( "00000000-0000-0000-0000-000000000003" ), nameof( Archived ) );

    public PostStatus( Guid id, string name )
        : base( id, name ) { }
}
