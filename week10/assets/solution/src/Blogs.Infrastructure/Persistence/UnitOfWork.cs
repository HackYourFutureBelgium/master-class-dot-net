namespace Blogs.Infrastructure.Persistence;

[ ExcludeFromCodeCoverage ]
public class UnitOfWork( Context context, IEventPublisher eventPublisher )
    : RootBlocks.Persistence.EntityFramework.UnitOfWork( context, eventPublisher )
{
    // public override void RegisterDirty( IAggregateRoot aggregateRoot )
    // {
    //     base.RegisterDirty( aggregateRoot );
    //
    //     if ( aggregateRoot is not Post )
    //         return;
    //
    //     var comments = context.ChangeTracker.Entries< Comment >();
    //     var commentsToDelete = comments.Where( ee => ee.Entity.Post == null );
    //
    //     foreach ( var comment in commentsToDelete )
    //         context.Remove( comment.Entity );
    // }
}
