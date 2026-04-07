namespace Blogs.Infrastructure.Persistence.Configurations.PostAggregate;

[ ExcludeFromCodeCoverage ]
public class TagConfiguration : ValueObjectConfiguration< Tag >
{
    public override void Configure( EntityTypeBuilder< Tag > builder )
    {
        base.Configure( builder );

        builder.Property( t => t.Value )
               .HasMaxLength( 256 )
               .IsRequired();

        builder.HasOne< Post >()
               .WithMany( p => p.Tags )
               .HasForeignKey( nameof( PostId ) )
               .OnDelete( DeleteBehavior.Cascade )
               .IsRequired();
    }
}
