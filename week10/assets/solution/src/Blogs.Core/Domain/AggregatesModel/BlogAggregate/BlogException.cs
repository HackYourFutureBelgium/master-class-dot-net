namespace Blogs.Core.Domain.AggregatesModel.BlogAggregate;

public class BlogException : DomainException
{
    public BlogException( BlogExceptionCode code, Exception? innerException = null )
        : base( code, innerException ) { }
}
