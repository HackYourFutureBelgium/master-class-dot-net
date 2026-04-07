namespace Blogs.Application.Commands.Blogs.Handlers;

public class DeleteBlogCommandHandler(
    ILogger< DeleteBlogCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IBlogRepository blogRepository
) : IRequestHandler< DeleteBlogCommand >
{
    private readonly ILogger< DeleteBlogCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IBlogRepository _blogRepository = blogRepository
                                                    ?? throw new ArgumentNullException( nameof( blogRepository ) );

    public async Task Handle( DeleteBlogCommand request, CancellationToken cancellationToken )
    {
        var blog = await _blogRepository.GetByIdAsync( request.BlogId, cancellationToken );
        _unitOfWork.RegisterDeleted( blog );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
