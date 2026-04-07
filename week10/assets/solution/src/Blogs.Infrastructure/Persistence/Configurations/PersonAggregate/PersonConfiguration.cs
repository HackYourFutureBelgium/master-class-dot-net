namespace Blogs.Infrastructure.Persistence.Configurations.PersonAggregate;

[ ExcludeFromCodeCoverage ]
public class PersonConfiguration : SoftDeletableEntityConfiguration< Person, PersonId >
{
    public override void Configure( EntityTypeBuilder< Person > builder )
    {
        base.Configure( builder );

        builder.Property( p => p.FullName )
               .HasMaxLength( 1024 )
               .IsRequired();
        builder.Property( p => p.ShortName )
               .HasMaxLength( 64 )
               .IsRequired( false );
        builder.Property( p => p.EmailAddress )
               .HasMaxLength( 320 )
               .IsRequired();

        builder.HasIndex( p => p.EmailAddress )
               .IsUnique();
    }
}
