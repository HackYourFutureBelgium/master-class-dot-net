namespace Blogs.Api.Model;

[ ExcludeFromCodeCoverage ]
public record CreateBlogRequestBody
{
    public string Title { get; set; } = null!;
    public PersonId OwnerId { get; set; } = null!;
    public string? Description { get; set; } = null!;
}
