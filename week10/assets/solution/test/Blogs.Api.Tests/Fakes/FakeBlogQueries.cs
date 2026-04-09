using Blogs.Core.Domain.AggregatesModel.BlogAggregate;
using Blogs.Core.ReadModel;
using RootBlocks.Exceptions;

namespace Blogs.Api.Tests.Fakes;

public class FakeBlogQueries( BlogDto? blog = null ) : IBlogQueries
{
    public Task< BlogDto > GetBlogAsync( BlogId blogId, CancellationToken ct = default )
    {
        if ( blog is null )
            throw new EntityNotFoundException< Blog >();

        return Task.FromResult( blog );
    }
}
