namespace Blogs.Application.Tests.Commands;

public class PostLifecycleCommandHandlerTests
{
    private static Post APost() => new( new BlogId(), "My title", "Some content." );

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_a_post_persists_a_new_post_entity()
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreatePostCommandHandler( NullLogger< CreatePostCommandHandler >.Instance, unitOfWork );
        var blogId = new BlogId();

        await handler.Handle( new CreatePostCommand( blogId, "My title", "Some content." ), default );

        var post = Assert.IsType< Post >( Assert.Single( unitOfWork.NewEntities ) );
        Assert.Equal( "My title", post.Title );
        Assert.Equal( blogId, post.BlogId );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Creating_a_post_returns_a_dto_in_draft_status()
    {
        var handler = new CreatePostCommandHandler(
            NullLogger< CreatePostCommandHandler >.Instance,
            new FakeUnitOfWork()
        );

        var dto = await handler.Handle( new CreatePostCommand( new BlogId(), "My title", "Some content." ), default );

        Assert.Equal( PostStatusDto.Draft, dto.PostStatus );
        Assert.Equal( "My title", dto.Title );
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_post_applies_the_patch_and_saves()
    {
        var post = APost();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new UpdatePostCommandHandler(
            NullLogger< UpdatePostCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );
        var patch = new JsonPatchDocument< Post >();
        patch.Replace( p => p.Title, "Updated Title" );

        await handler.Handle( new UpdatePostCommand( post.Id, patch ), default );

        Assert.Equal( "Updated Title", post.Title );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    // ── Publish ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Publishing_a_post_marks_it_as_published_and_saves()
    {
        var post = APost();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new PublishPostCommandHandler(
            NullLogger< PublishPostCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new PublishPostCommand( post.Id ), default );

        Assert.Equal( PostStatus.Published, post.PostStatus );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Publishing_a_post_that_does_not_exist_throws()
    {
        var post = APost();
        var handler = new PublishPostCommandHandler(
            NullLogger< PublishPostCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePostRepository( post )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Post > >(
            () => handler.Handle( new PublishPostCommand( new PostId() ), default )
        );
    }

    // ── Unpublish ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Unpublishing_a_post_reverts_it_to_draft_and_saves()
    {
        var post = APost();
        post.Publish();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new UnpublishPostCommandHandler(
            NullLogger< UnpublishPostCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new UnpublishPostCommand( post.Id ), default );

        Assert.Equal( PostStatus.Draft, post.PostStatus );
        Assert.Contains( post, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_post_marks_it_for_deletion_and_commits()
    {
        var post = APost();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeletePostCommandHandler(
            NullLogger< DeletePostCommandHandler >.Instance,
            unitOfWork,
            new FakePostRepository( post )
        );

        await handler.Handle( new DeletePostCommand( post.Id ), default );

        Assert.Contains( post, unitOfWork.DeletedEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_post_that_does_not_exist_throws()
    {
        var post = APost();
        var handler = new DeletePostCommandHandler(
            NullLogger< DeletePostCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePostRepository( post )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Post > >(
            () => handler.Handle( new DeletePostCommand( new PostId() ), default )
        );
    }
}
