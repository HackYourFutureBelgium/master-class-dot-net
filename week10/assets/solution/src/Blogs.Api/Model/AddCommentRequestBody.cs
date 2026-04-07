namespace Blogs.Api.Model;

public record AddCommentRequestBody
{
    public PersonId AuthorId { get; set; } = null!;
    public string Content { get; set; } = null!;
}
