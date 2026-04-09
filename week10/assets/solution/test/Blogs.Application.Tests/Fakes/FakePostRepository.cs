using Blogs.Core.Domain.AggregatesModel.PostAggregate;
using RootBlocks.Exceptions;

namespace Blogs.Application.Tests.Fakes;

public class FakePostRepository( Post post ) : IPostRepository
{
    public Task< Post > GetByIdAsync( PostId postId, CancellationToken ct = default )
    {
        if ( post.Id != postId )
            throw new EntityNotFoundException< Post >( nameof( Post.Id ), postId );

        return Task.FromResult( post );
    }
}
