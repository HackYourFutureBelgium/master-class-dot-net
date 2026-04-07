namespace Blogs.Application.Commands.Blogs;

public record UpdateBlogCommand( BlogId BlogId, JsonPatchDocument< Blog > BlogPatchDocument ) : IRequest;
