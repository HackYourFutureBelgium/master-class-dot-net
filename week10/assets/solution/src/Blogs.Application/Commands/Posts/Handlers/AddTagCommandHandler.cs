namespace Blogs.Application.Commands.Posts.Handlers;

public class AddTagCommandHandler(
    ILogger< AddTagCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< AddTagCommand >
{
    private readonly ILogger< AddTagCommandHandler > _logger = logger
                                                            ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( AddTagCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        post.AddTag( request.Tag );
        _unitOfWork.RegisterDirty( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
