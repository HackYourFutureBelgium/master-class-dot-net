namespace Blogs.Infrastructure.Persistence.Configurations.PostAggregate;

[ ExcludeFromCodeCoverage ]
public class PostStatusConfiguration : EnumerationConfiguration< PostStatus >
{
    public override void Configure( EntityTypeBuilder< PostStatus > builder )
    {
        base.Configure( builder );
        builder.ToTable( nameof( PostStatus ) );
    }
}
