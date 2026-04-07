namespace Blogs.Application.Commands.Posts;

public record DeleteTagCommand( PostId PostId, string Tag ) : IRequest;
