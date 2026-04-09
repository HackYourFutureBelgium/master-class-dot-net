namespace Blogs.Api.Model;

[ ExcludeFromCodeCoverage ]
public record CreatePostRequestBody
{
    public BlogId BlogId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}
