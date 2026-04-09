namespace Blogs.Application.Tests.Commands;

public class PostCommentCommandHandlerTests
{
    private static Post APost() => new( new BlogId(), "My title", "Some content." );

    [Fact]
    public async Task Adding_a_comment_to_a_post_saves_the_updated_post()
    {
        var post = APost();
        var authorId = new PersonId();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AddCommentCommandHandler(
            NullLogger< AddCommentCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new AddCommentCommand( post.Id, authorId, "Great post!" ), default );

        Assert.Single( post.Comments );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_comment_from_a_post_saves_the_updated_post()
    {
        var post = APost();
        var comment = post.AddComment( new PersonId(), "Great post!" );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeleteCommentCommandHandler(
            NullLogger< DeleteCommentCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new DeleteCommentCommand( post.Id, comment.Id ), default );

        Assert.Empty( post.Comments );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_comment_that_does_not_exist_throws()
    {
        var post = APost();
        var handler = new DeleteCommentCommandHandler(
            NullLogger< DeleteCommentCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePostRepository( post )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Comment > >(
            () => handler.Handle( new DeleteCommentCommand( post.Id, new CommentId() ), default )
        );
    }
}
