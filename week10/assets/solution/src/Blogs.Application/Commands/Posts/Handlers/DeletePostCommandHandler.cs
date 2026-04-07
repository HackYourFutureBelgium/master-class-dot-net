namespace Blogs.Application.Commands.Posts.Handlers;

public class DeletePostCommandHandler(
    ILogger< DeletePostCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< DeletePostCommand >
{
    private readonly ILogger< DeletePostCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( DeletePostCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        _unitOfWork.RegisterDeleted( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
