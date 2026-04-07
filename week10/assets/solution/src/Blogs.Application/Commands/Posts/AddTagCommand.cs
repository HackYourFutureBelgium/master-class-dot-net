namespace Blogs.Application.Commands.Posts;

public record AddTagCommand( PostId PostId, string Tag ) : IRequest;
