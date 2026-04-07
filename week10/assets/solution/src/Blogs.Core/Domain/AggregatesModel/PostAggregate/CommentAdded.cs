namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class CommentAdded( PostId postId ) : DomainEvent
{
    public PostId PostId { get; } = postId;
}
