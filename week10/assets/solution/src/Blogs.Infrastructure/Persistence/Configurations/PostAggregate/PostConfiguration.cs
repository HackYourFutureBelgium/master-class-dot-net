namespace Blogs.Infrastructure.Persistence.Configurations.PostAggregate;

[ ExcludeFromCodeCoverage ]
public class PostConfiguration : EntityConfiguration< Post, PostId >
{
    public override void Configure( EntityTypeBuilder< Post > builder )
    {
        base.Configure( builder );

        builder.Property( p => p.Title )
               .HasMaxLength( 256 )
               .IsRequired();
        builder.Property( p => p.Content )
               .HasMaxLength( 4096 )
               .IsRequired();
        builder.Property( p => p.PublishedOn )
               .IsRequired( false );
        builder.Property( p => p.ArchivedOn )
               .IsRequired( false );
        builder.Property( p => p.BlogId )
               .HasConversion( new IdentityToGuidConverter< BlogId >() )
               .IsRequired();
        builder.Property( p => p.PostStatusId )
               .IsRequired();

        var navigationToComments = builder.Metadata.FindNavigation( nameof( Post.Comments ) );
        navigationToComments!.SetPropertyAccessMode( PropertyAccessMode.Field );
        var navigationToTags = builder.Metadata.FindNavigation( nameof( Post.Tags ) );
        navigationToTags!.SetPropertyAccessMode( PropertyAccessMode.Field );

        builder.HasOne< Blog >()
               .WithMany()
               .HasForeignKey( p => p.BlogId )
               .OnDelete( DeleteBehavior.Cascade )
               .IsRequired();
        builder.HasOne( p => p.PostStatus )
               .WithMany()
               .HasForeignKey( p => p.PostStatusId )
               .OnDelete( DeleteBehavior.Restrict )
               .IsRequired();

        builder.HasIndex( np => np.Title );
    }
}
