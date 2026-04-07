namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public interface IPostRepository : IRepository< Post >
{
    Task< Post > GetByIdAsync( PostId postId, CancellationToken cancellationToken = default );
}
