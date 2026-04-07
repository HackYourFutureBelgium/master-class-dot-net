namespace Blogs.Core.ReadModel;

public interface IPostQueries
{
    Task< (IEnumerable< PostDto >, uint) > FindPostsAsync(
        string? searchTerm,
        IEnumerable< string >? tags,
        uint pageIndex = 1,
        uint pageSize = 10,
        CancellationToken cancellationToken = default
    );

    Task< PostDto > GetPostAsync( PostId postId, CancellationToken cancellationToken = default );
}
