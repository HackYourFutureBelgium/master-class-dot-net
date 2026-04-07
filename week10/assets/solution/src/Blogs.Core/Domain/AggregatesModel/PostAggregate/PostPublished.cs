namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class PostPublished( PostId postId ) : DomainEvent
{
    public PostId PostId { get; } = postId;
}
