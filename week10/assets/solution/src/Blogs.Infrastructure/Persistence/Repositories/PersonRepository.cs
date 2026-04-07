namespace Blogs.Infrastructure.Persistence.Repositories;

[ ExcludeFromCodeCoverage ]
public class PersonRepository( ILogger< PersonRepository > logger, Context context ) : IPersonRepository
{
    private readonly ILogger< PersonRepository > _logger = logger
                                                        ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly Context _context = context
                                     ?? throw new ArgumentNullException( nameof( context ) );

    public async Task< Person > GetByIdAsync( PersonId personId, CancellationToken cancellationToken = default )
    {
        var person = await _context.People.SingleOrDefaultAsync( p => p.Id == personId, cancellationToken );

        if ( person is null )
            throw new EntityNotFoundException< Person >( nameof( Person.Id ), personId );

        return person;
    }
}
