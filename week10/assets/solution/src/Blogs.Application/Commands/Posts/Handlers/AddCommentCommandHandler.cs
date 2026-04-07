namespace Blogs.Application.Commands.Posts.Handlers;

public class AddCommentCommandHandler(
    ILogger< AddCommentCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< AddCommentCommand >
{
    private readonly ILogger< AddCommentCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( AddCommentCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        post.AddComment( request.AuthorId, request.Content );
        _unitOfWork.RegisterDirty( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
