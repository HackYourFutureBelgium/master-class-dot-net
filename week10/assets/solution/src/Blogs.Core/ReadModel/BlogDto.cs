namespace Blogs.Core.ReadModel;

public class BlogDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModifiedOn { get; set; }

    public static explicit operator BlogDto( Blog blog )
    {
        return new BlogDto
        {
            Id = blog.Id.Value,
            Title = blog.Title,
            Description = blog.Description,
            OwnerId = blog.OwnerId.Value,
            CreatedOn = blog.CreatedOn,
            LastModifiedOn = blog.LastModifiedOn
        };
    }
}
