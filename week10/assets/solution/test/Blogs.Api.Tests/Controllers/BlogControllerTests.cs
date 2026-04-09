namespace Blogs.Api.Tests.Controllers;

public class BlogControllerTests
{
    private static BlogDto ABlogDto() => new()
    {
        Id = Guid.NewGuid(), Title = "My Blog", OwnerId = Guid.NewGuid()
    };

    private static BlogController Controller( FakeMediator? mediator = null, FakeBlogQueries? queries = null )
        => new(
            NullLogger< BlogController >.Instance,
            mediator ?? new FakeMediator(),
            queries  ?? new FakeBlogQueries()
        );

    // ── GET /{id} ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Getting_a_blog_returns_200_with_the_blog_dto()
    {
        var dto = ABlogDto();
        var result = await Controller( queries: new FakeBlogQueries( dto ) )
            .GetBlog( new BlogId(), default );

        var ok = Assert.IsType< OkObjectResult >( result );
        Assert.Same( dto, ok.Value );
    }

    [Fact]
    public async Task Getting_a_blog_that_does_not_exist_returns_404()
    {
        var result = await Controller().GetBlog( new BlogId(), default );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── POST / ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_a_blog_returns_201_pointing_to_the_new_blog()
    {
        var dto = ABlogDto();
        var mediator = new FakeMediator( dto );
        var result = await Controller( mediator ).CreateBlog(
            new CreateBlogRequestBody { Title = "My Blog", OwnerId = new PersonId() },
            default
        );

        var created = Assert.IsType< CreatedAtActionResult >( result );
        Assert.Equal( nameof( BlogController.GetBlog ), created.ActionName );
        Assert.Same( dto, created.Value );
    }

    [Fact]
    public async Task Creating_a_blog_dispatches_the_correct_command()
    {
        var ownerId = new PersonId();
        var mediator = new FakeMediator( ABlogDto() );
        await Controller( mediator ).CreateBlog(
            new CreateBlogRequestBody { Title = "My Blog", OwnerId = ownerId, Description = "A description." },
            default
        );

        var command = Assert.IsType< CreateBlogCommand >( mediator.CapturedRequest );
        Assert.Equal( "My Blog", command.Title );
        Assert.Equal( ownerId, command.OwnerId );
        Assert.Equal( "A description.", command.Description );
    }

    // ── PATCH /{id} ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_blog_returns_204()
    {
        var result = await Controller().UpdateBlog(
            new BlogId(), new JsonPatchDocument< Blog >(), default
        );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Updating_a_blog_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Blog >() );
        var result = await Controller( mediator ).UpdateBlog(
            new BlogId(), new JsonPatchDocument< Blog >(), default
        );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── DELETE /{id} ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_blog_returns_204()
    {
        var result = await Controller().DeleteBlog( new BlogId(), default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Deleting_a_blog_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Blog >() );
        var result = await Controller( mediator ).DeleteBlog( new BlogId(), default );
        Assert.IsType< NotFoundResult >( result );
    }
}
