namespace Blogs.Application.Commands.People.Handlers;

public class DeletePersonCommandHandler(
    ILogger< DeletePersonCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPersonRepository personRepository
) : IRequestHandler< DeletePersonCommand >
{
    private readonly ILogger< DeletePersonCommandHandler > _logger = logger
                                                                  ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                           ?? throw new ArgumentNullException( nameof( unitOfWork ) );
    private readonly IPersonRepository _personRepository = personRepository
                                                       ?? throw new ArgumentNullException( nameof( personRepository ) );

    public async Task Handle( DeletePersonCommand request, CancellationToken cancellationToken )
    {
        var person = await _personRepository.GetByIdAsync( request.PersonId, cancellationToken );
        _unitOfWork.RegisterDeleted( person );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
