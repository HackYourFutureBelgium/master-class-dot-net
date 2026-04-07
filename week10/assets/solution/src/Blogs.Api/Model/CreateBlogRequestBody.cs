namespace Blogs.Api.Model;

public record CreateBlogRequestBody
{
    public string Title { get; set; } = null!;
    public PersonId OwnerId { get; set; } = null!;
    public string? Description { get; set; } = null!;
}
