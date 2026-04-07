namespace Blogs.Infrastructure.Persistence.Configurations.BlogAggregate;

[ ExcludeFromCodeCoverage ]
public class BlogConfiguration : EntityConfiguration< Blog, BlogId >
{
    public override void Configure( EntityTypeBuilder< Blog > builder )
    {
        base.Configure( builder );

        builder.Property( b => b.Title )
               .HasMaxLength( 256 )
               .IsRequired();
        builder.Property( b => b.Description )
               .HasMaxLength( 1024 )
               .IsRequired( false );
        builder.Property( b => b.OwnerId )
               .HasConversion( new IdentityToGuidConverter< PersonId >() )
               .IsRequired();

        builder.HasOne< Person >()
               .WithMany()
               .HasForeignKey( p => p.OwnerId )
               .OnDelete( DeleteBehavior.Restrict )
               .IsRequired();

        builder.HasIndex( b => b.Title );
    }
}
