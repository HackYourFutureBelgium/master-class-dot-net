namespace Blogs.Application.Tests.Commands;

public class BlogCommandHandlerTests
{
    private static readonly PersonId OwnerId = new();

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_a_blog_persists_a_new_blog_entity()
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateBlogCommandHandler( NullLogger< CreateBlogCommandHandler >.Instance, unitOfWork );

        await handler.Handle( new CreateBlogCommand( "My Blog", OwnerId, "A description." ), default );

        var blog = Assert.IsType< Blog >( Assert.Single( unitOfWork.NewEntities ) );
        Assert.Equal( "My Blog", blog.Title );
        Assert.Equal( OwnerId, blog.OwnerId );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Creating_a_blog_returns_a_dto_matching_the_given_data()
    {
        var handler = new CreateBlogCommandHandler(
            NullLogger< CreateBlogCommandHandler >.Instance,
            new FakeUnitOfWork()
        );

        var dto = await handler.Handle( new CreateBlogCommand( "My Blog", OwnerId, "A description." ), default );

        Assert.Equal( "My Blog", dto.Title );
        Assert.Equal( OwnerId.Value, dto.OwnerId );
        Assert.Equal( "A description.", dto.Description );
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_blog_applies_the_patch_and_saves()
    {
        var blog = new Blog( "My Blog", OwnerId );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new UpdateBlogCommandHandler(
            NullLogger< UpdateBlogCommandHandler >.Instance,
            unitOfWork,
            new FakeBlogRepository( blog )
        );
        var patch = new JsonPatchDocument< Blog >();
        patch.Replace( b => b.Title, "Updated Title" );

        await handler.Handle( new UpdateBlogCommand( blog.Id, patch ), default );

        Assert.Equal( "Updated Title", blog.Title );
        Assert.Contains( blog, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Updating_a_blog_that_does_not_exist_throws()
    {
        var blog = new Blog( "My Blog", OwnerId );
        var handler = new UpdateBlogCommandHandler(
            NullLogger< UpdateBlogCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakeBlogRepository( blog )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Blog > >(
            () => handler.Handle( new UpdateBlogCommand( new BlogId(), new JsonPatchDocument< Blog >() ), default )
        );
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_blog_marks_it_for_deletion_and_commits()
    {
        var blog = new Blog( "My Blog", OwnerId );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeleteBlogCommandHandler(
            NullLogger< DeleteBlogCommandHandler >.Instance,
            unitOfWork,
            new FakeBlogRepository( blog )
        );

        await handler.Handle( new DeleteBlogCommand( blog.Id ), default );

        Assert.Contains( blog, unitOfWork.DeletedEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_blog_that_does_not_exist_throws()
    {
        var blog = new Blog( "My Blog", OwnerId );
        var handler = new DeleteBlogCommandHandler(
            NullLogger< DeleteBlogCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakeBlogRepository( blog )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Blog > >(
            () => handler.Handle( new DeleteBlogCommand( new BlogId() ), default )
        );
    }
}
