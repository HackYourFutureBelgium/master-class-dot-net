namespace Blogs.Application.Commands.Posts;

public record DeletePostCommand( PostId PostId ) : IRequest;
