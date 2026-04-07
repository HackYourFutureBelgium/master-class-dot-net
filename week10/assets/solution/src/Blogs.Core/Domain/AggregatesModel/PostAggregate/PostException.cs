namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class PostException : DomainException
{
    public PostException( PostExceptionCode code, Exception? innerException = null )
        : base( code, innerException ) { }
}
