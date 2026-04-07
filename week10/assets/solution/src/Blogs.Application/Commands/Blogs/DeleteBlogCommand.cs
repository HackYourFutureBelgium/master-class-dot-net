namespace Blogs.Application.Commands.Blogs;

public record DeleteBlogCommand( BlogId BlogId ) : IRequest;
