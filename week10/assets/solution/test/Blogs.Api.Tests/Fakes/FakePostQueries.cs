using Blogs.Core.Domain.AggregatesModel.PostAggregate;
using Blogs.Core.ReadModel;
using RootBlocks.Exceptions;

namespace Blogs.Api.Tests.Fakes;

public class FakePostQueries( PostDto? post = null ) : IPostQueries
{
    public Task< PostDto > GetPostAsync( PostId postId, CancellationToken ct = default )
    {
        if ( post is null )
            throw new EntityNotFoundException< Post >();

        return Task.FromResult( post );
    }

    public Task< (IEnumerable< PostDto >, uint) > FindPostsAsync(
        string? searchTerm,
        IEnumerable< string >? tags,
        uint pageIndex = 1,
        uint pageSize = 10,
        CancellationToken ct = default
    )
    {
        IEnumerable< PostDto > results = post is null ? [ ] : [ post ];
        return Task.FromResult( ( results, (uint)results.Count() ) );
    }
}
