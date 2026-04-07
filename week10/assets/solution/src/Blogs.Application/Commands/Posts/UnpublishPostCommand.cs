namespace Blogs.Application.Commands.Posts;

public record UnpublishPostCommand( PostId PostId ) : IRequest;
