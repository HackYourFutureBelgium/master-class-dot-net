using Blogs.Core.Domain.AggregatesModel.PersonAggregate;
using Blogs.Core.ReadModel;
using RootBlocks.Exceptions;
using RootBlocks.Pagination;

namespace Blogs.Api.Tests.Fakes;

public class FakePersonQueries( PersonDto? person = null ) : IPersonQueries
{
    public Task< PersonDto > GetPersonAsync( PersonId personId, CancellationToken ct = default )
    {
        if ( person is null )
            throw new EntityNotFoundException< Person >();

        return Task.FromResult( person );
    }

    public Task< (IEnumerable< PersonDto >, uint) > FindPeopleAsync(
        string? searchTerm,
        uint pageIndex = 1,
        uint pageSize = 10,
        string sortColumn = "fullName",
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken ct = default
    )
    {
        IEnumerable< PersonDto > results = person is null ? [ ] : [ person ];
        return Task.FromResult( ( results, (uint)results.Count() ) );
    }
}
