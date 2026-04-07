namespace Blogs.Application.Commands.Posts;

public record UpdatePostCommand( PostId PostId, JsonPatchDocument< Post > PostPatchDocument ) : IRequest;
