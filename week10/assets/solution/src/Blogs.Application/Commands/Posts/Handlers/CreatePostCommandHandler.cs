namespace Blogs.Application.Commands.Posts.Handlers;

public class CreatePostCommandHandler(
    ILogger< CreatePostCommandHandler > logger,
    IUnitOfWork unitOfWork
) : IRequestHandler< CreatePostCommand, PostDto >
{
    private readonly ILogger< CreatePostCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );

    public async Task< PostDto > Handle( CreatePostCommand request, CancellationToken cancellationToken )
    {
        var post = new Post( request.BlogId, request.Title, request.Content );
        _unitOfWork.RegisterNew( post );
        await _unitOfWork.CommitAsync( cancellationToken );
        var postDto = ( PostDto )post;
        return postDto;
    }
}
