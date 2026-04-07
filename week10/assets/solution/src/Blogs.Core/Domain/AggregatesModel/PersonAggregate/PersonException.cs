namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

public class PersonException : DomainException
{
    public PersonException( PersonExceptionCode code, Exception? innerException = null )
        : base( code, innerException ) { }
}
