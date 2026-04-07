namespace Blogs.Core.Domain.AggregatesModel.BlogAggregate;

public interface IBlogRepository : IRepository< Blog >
{
    Task< Blog > GetByIdAsync( BlogId blogId, CancellationToken cancellationToken = default );
}
