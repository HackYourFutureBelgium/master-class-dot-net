namespace Blogs.Api.Model;

[ ExcludeFromCodeCoverage ]
public record RegisterPersonRequestBody
{
    public string FullName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
}
