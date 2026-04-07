namespace Blogs.Application.Commands.People.Handlers;

public class UpdatePersonCommandHandler(
    ILogger< UpdatePersonCommandHandler > logger,
    IUnitOfWork unitOfWork,
    IPersonRepository personRepository
) : IRequestHandler< UpdatePersonCommand >
{
    private readonly ILogger< UpdatePersonCommandHandler > _logger = logger
                                                                  ?? throw new ArgumentNullException( nameof( logger ) );

    private readonly IUnitOfWork _unitOfWork = unitOfWork
                                            ?? throw new ArgumentNullException( nameof( unitOfWork ) );

    private readonly IPersonRepository _personRepository = personRepository
                                                        ?? throw new ArgumentNullException( nameof( personRepository ) );

    public async Task Handle( UpdatePersonCommand request, CancellationToken cancellationToken )
    {
        var person = await _personRepository.GetByIdAsync( request.PersonId, cancellationToken );
        var dto = ( PersonDto )person;
        request.PersonPatchDocument.ApplyTo( dto );
        person.EmailAddress = dto.EmailAddress;
        person.FullName = dto.FullName;
        person.ShortName = dto.ShortName;
        _unitOfWork.RegisterDirty( person );
        await _unitOfWork.CommitAsync( cancellationToken );
    }
}
