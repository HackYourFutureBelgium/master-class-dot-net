namespace Blogs.Infrastructure.Persistence;

[ ExcludeFromCodeCoverage ]
public class UnitOfWork( Context context, IEventPublisher eventPublisher )
    : RootBlocks.Persistence.EntityFramework.UnitOfWork( context, eventPublisher );
