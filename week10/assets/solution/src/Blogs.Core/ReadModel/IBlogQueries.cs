namespace Blogs.Core.ReadModel;

public interface IBlogQueries
{
    Task<BlogDto> GetBlogAsync( BlogId blogId, CancellationToken cancellationToken = default );
}
