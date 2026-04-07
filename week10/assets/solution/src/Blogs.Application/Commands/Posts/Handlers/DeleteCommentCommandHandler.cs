namespace Blogs.Application.Commands.Posts.Handlers;

public class DeleteCommentCommandHandler(
    ILogger< DeleteCommentCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< DeleteCommentCommand >
{
    private readonly ILogger< DeleteCommentCommandHandler > _logger = logger
                                                                   ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( DeleteCommentCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        var comment = post.Comments.SingleOrDefault( c => c.Id == request.CommentId );

        if ( comment is null )
            throw new EntityNotFoundException< Comment >( nameof( Comment ), request.CommentId );

        post.DeleteComment( comment );
        _unitOfWork.RegisterDirty( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
