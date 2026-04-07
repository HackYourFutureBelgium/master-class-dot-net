namespace Blogs.Application.Commands.Posts.Handlers;

public class DeleteTagCommandHandler(
    ILogger< DeleteTagCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPostRepository postRepository
) : IRequestHandler< DeleteTagCommand >
{
    private readonly ILogger< DeleteTagCommandHandler > _logger = logger
                                                               ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPostRepository _postRepository = postRepository
                                                    ?? throw new ArgumentNullException( nameof( postRepository ) );

    public async Task Handle( DeleteTagCommand request, CancellationToken cancellationToken )
    {
        var post = await _postRepository.GetByIdAsync( request.PostId, cancellationToken );
        var tag = post.Tags.SingleOrDefault( t => t.Value == request.Tag );

        if ( tag is null )
            throw new EntityNotFoundException< Tag >( nameof( Tag ), request.Tag );

        post.DeleteTag( tag );
        _unitOfWork.RegisterDirty( post );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
