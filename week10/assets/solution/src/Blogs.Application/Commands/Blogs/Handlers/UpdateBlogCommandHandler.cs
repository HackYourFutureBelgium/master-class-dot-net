namespace Blogs.Application.Commands.Blogs.Handlers;

public class UpdateBlogCommandHandler(
    ILogger< UpdateBlogCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IBlogRepository blogRepository
) : IRequestHandler< UpdateBlogCommand >
{
    private readonly ILogger< UpdateBlogCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IBlogRepository _blogRepository = blogRepository
                                                    ?? throw new ArgumentNullException( nameof( blogRepository ) );

    public async Task Handle( UpdateBlogCommand request, CancellationToken cancellationToken )
    {
        var blog = await _blogRepository.GetByIdAsync( request.BlogId, cancellationToken );
        request.BlogPatchDocument.ApplyTo( blog );
        _unitOfWork.RegisterDirty( blog );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
