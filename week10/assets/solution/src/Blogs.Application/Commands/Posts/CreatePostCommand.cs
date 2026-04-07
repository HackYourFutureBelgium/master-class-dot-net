namespace Blogs.Application.Commands.Posts;

public record CreatePostCommand( BlogId BlogId, string Title, string Content ) : IRequest< PostDto >;
