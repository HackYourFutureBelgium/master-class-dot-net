namespace Blogs.Api.Model;

public record CreatePostRequestBody
{
    public BlogId BlogId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}
