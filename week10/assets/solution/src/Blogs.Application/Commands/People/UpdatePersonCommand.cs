namespace Blogs.Application.Commands.People;

public record UpdatePersonCommand( PersonId PersonId, JsonPatchDocument< PersonDto > PersonPatchDocument ) : IRequest;
