namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

public interface IPersonRepository : IRepository< Person >
{
    Task< Person > GetByIdAsync( PersonId personId, CancellationToken cancellationToken = default );
}
