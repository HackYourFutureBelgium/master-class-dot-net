namespace Blogs.Application.Commands.People;

public record DeletePersonCommand( PersonId PersonId ) : IRequest;
