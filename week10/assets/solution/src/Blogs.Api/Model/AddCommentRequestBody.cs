namespace Blogs.Api.Model;

[ ExcludeFromCodeCoverage ]
public record AddCommentRequestBody
{
    public PersonId AuthorId { get; set; } = null!;
    public string Content { get; set; } = null!;
}
