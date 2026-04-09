namespace Blogs.Application.Tests.Commands;

public class PostTagCommandHandlerTests
{
    private static Post APost() => new( new BlogId(), "My title", "Some content." );

    [Fact]
    public async Task Adding_a_tag_to_a_post_saves_the_updated_post()
    {
        var post = APost();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new AddTagCommandHandler(
            NullLogger< AddTagCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new AddTagCommand( post.Id, "dotnet" ), default );

        Assert.Single( post.Tags );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_tag_from_a_post_saves_the_updated_post()
    {
        var post = APost();
        post.AddTag( "dotnet" );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeleteTagCommandHandler(
            NullLogger< DeleteTagCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new DeleteTagCommand( post.Id, "dotnet" ), default );

        Assert.Empty( post.Tags );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_tag_that_does_not_exist_throws()
    {
        var post = APost();
        var handler = new DeleteTagCommandHandler(
            NullLogger< DeleteTagCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePostRepository( post )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Tag > >(
            () => handler.Handle( new DeleteTagCommand( post.Id, "nonexistent" ), default )
        );
    }
}
