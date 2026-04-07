namespace Blogs.Application.Commands.People;

public record RegisterPersonCommand( string Fullname, string EmailAddress ) : IRequest< PersonDto >;
