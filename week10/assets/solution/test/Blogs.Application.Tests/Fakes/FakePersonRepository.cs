using Blogs.Core.Domain.AggregatesModel.PersonAggregate;
using RootBlocks.Exceptions;

namespace Blogs.Application.Tests.Fakes;

public class FakePersonRepository( Person person ) : IPersonRepository
{
    public Task< Person > GetByIdAsync( PersonId personId, CancellationToken ct = default )
    {
        if ( person.Id != personId )
            throw new EntityNotFoundException< Person >( nameof( Person.Id ), personId );

        return Task.FromResult( person );
    }
}
