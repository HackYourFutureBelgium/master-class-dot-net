namespace Blogs.Application.Commands.People.Handlers;

public class RegisterPersonCommandHandler(
    ILogger< RegisterPersonCommandHandler > logger,
    IUnitOfWork unitOfWork
) : IRequestHandler< RegisterPersonCommand, PersonDto >
{
    private readonly ILogger< RegisterPersonCommandHandler > _logger = logger
                                                                    ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );

    public async Task< PersonDto > Handle(
        RegisterPersonCommand request,
        CancellationToken cancellationToken
    )
    {
        var person = new Person( request.Fullname, request.EmailAddress );
        _unitOfWork.RegisterNew( person );
        await _unitOfWork.CommitAsync( cancellationToken );
        var dto = ( PersonDto )person;
        return dto;
    }
}
