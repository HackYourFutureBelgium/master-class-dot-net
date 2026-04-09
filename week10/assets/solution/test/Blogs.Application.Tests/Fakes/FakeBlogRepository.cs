using Blogs.Core.Domain.AggregatesModel.BlogAggregate;
using RootBlocks.Exceptions;

namespace Blogs.Application.Tests.Fakes;

public class FakeBlogRepository( Blog blog ) : IBlogRepository
{
    public Task< Blog > GetByIdAsync( BlogId blogId, CancellationToken ct = default )
    {
        if ( blog.Id != blogId )
            throw new EntityNotFoundException< Blog >( nameof( Blog.Id ), blogId );

        return Task.FromResult( blog );
    }
}
