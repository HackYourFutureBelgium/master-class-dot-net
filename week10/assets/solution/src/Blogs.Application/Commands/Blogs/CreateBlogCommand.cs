namespace Blogs.Application.Commands.Blogs;

public record CreateBlogCommand( string Title, PersonId OwnerId, string? Description ) : IRequest< BlogDto >;
