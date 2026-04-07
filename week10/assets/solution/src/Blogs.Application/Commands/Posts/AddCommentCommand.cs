namespace Blogs.Application.Commands.Posts;

public record AddCommentCommand( PostId PostId, PersonId AuthorId, string Content ) : IRequest;
