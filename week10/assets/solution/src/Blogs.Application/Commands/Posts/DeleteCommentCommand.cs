namespace Blogs.Application.Commands.Posts;

public record DeleteCommentCommand( PostId PostId, CommentId CommentId ) : IRequest;
