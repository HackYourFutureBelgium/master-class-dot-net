namespace Blogs.Application.Commands.Posts;

public record PublishPostCommand( PostId PostId ) : IRequest;
