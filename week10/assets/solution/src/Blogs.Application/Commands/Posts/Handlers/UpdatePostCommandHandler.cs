namespace Blogs.Application.Commands.Posts.Handlers;

public class UpdatePostCommandHandler(
    ILogger< UpdatePostCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< UpdatePostCommand >
{
    private readonly ILogger< UpdatePostCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( UpdatePostCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        request.PostPatchDocument.ApplyTo( post );
        _unitOfWork.RegisterDirty( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
