namespace Blogs.Application.Commands.Blogs.Handlers;

public class CreateBlogCommandHandler(
    ILogger< CreateBlogCommandHandler > logger,
    IUnitOfWork unitOfWork
) : IRequestHandler< CreateBlogCommand, BlogDto >
{
    private readonly ILogger< CreateBlogCommandHandler > _logger = logger
                                                                ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );

    public async Task< BlogDto > Handle( CreateBlogCommand request, CancellationToken cancellationToken )
    {
        var blog = new Blog( request.Title, request.OwnerId, request.Description );
        _unitOfWork.RegisterNew( blog );
        await _unitOfWork.CommitAsync( cancellationToken );
        var blogDto = ( BlogDto )blog;
        return blogDto;
    }
}
