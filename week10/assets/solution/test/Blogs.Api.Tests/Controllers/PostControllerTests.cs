namespace Blogs.Api.Tests.Controllers;

public class PostControllerTests
{
    private static PostDto APostDto() => new()
    {
        Id = Guid.NewGuid(), Title = "My Post", Content = "Some content.", PostStatus = PostStatusDto.Draft
    };

    private static PostController Controller( FakeMediator? mediator = null, FakePostQueries? queries = null )
        => new(
            NullLogger< PostController >.Instance,
            mediator ?? new FakeMediator(),
            queries  ?? new FakePostQueries()
        );

    // ── GET /{id} ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Getting_a_post_returns_200_with_the_post_dto()
    {
        var dto = APostDto();
        var result = await Controller( queries: new FakePostQueries( dto ) )
            .GetPost( new PostId(), default );

        var ok = Assert.IsType< OkObjectResult >( result );
        Assert.Same( dto, ok.Value );
    }

    [Fact]
    public async Task Getting_a_post_that_does_not_exist_returns_404()
    {
        var result = await Controller().GetPost( new PostId(), default );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── GET / ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Finding_posts_returns_200_with_a_paged_result()
    {
        var dto = APostDto();
        var result = await Controller( queries: new FakePostQueries( dto ) )
            .FindPosts( cancellationToken: default );

        var ok = Assert.IsType< OkObjectResult >( result );
        var paged = Assert.IsType< PagedResult< PostDto > >( ok.Value );
        Assert.Single( paged.Items );
    }

    // ── POST / ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Creating_a_post_returns_201_pointing_to_the_new_post()
    {
        var dto = APostDto();
        var mediator = new FakeMediator( dto );
        var result = await Controller( mediator ).CreatePost(
            new CreatePostRequestBody { BlogId = new BlogId(), Title = "My Post", Content = "Some content." },
            default
        );

        var created = Assert.IsType< CreatedAtActionResult >( result );
        Assert.Equal( nameof( PostController.GetPost ), created.ActionName );
        Assert.Same( dto, created.Value );
    }

    [Fact]
    public async Task Creating_a_post_dispatches_the_correct_command()
    {
        var blogId = new BlogId();
        var mediator = new FakeMediator( APostDto() );
        await Controller( mediator ).CreatePost(
            new CreatePostRequestBody { BlogId = blogId, Title = "My Post", Content = "Some content." },
            default
        );

        var command = Assert.IsType< CreatePostCommand >( mediator.CapturedRequest );
        Assert.Equal( blogId, command.BlogId );
        Assert.Equal( "My Post", command.Title );
        Assert.Equal( "Some content.", command.Content );
    }

    // ── PATCH /{id} ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_post_returns_204()
    {
        var result = await Controller().UpdatePost(
            new PostId(), new JsonPatchDocument< Post >(), default
        );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Updating_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).UpdatePost(
            new PostId(), new JsonPatchDocument< Post >(), default
        );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── POST /{id}/tags/{tag} ─────────────────────────────────────────────────

    [Fact]
    public async Task Adding_a_tag_returns_204()
    {
        var result = await Controller().AddTag( new PostId(), "dotnet", default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Adding_a_tag_to_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).AddTag( new PostId(), "dotnet", default );
        Assert.IsType< NotFoundResult >( result );
    }

    [Fact]
    public async Task Adding_a_tag_dispatches_the_correct_command()
    {
        var postId = new PostId();
        var mediator = new FakeMediator();
        await Controller( mediator ).AddTag( postId, "dotnet", default );

        var command = Assert.IsType< AddTagCommand >( mediator.CapturedRequest );
        Assert.Equal( postId, command.PostId );
        Assert.Equal( "dotnet", command.Tag );
    }

    // ── DELETE /{id}/tags/{tag} ───────────────────────────────────────────────

    [Fact]
    public async Task Removing_a_tag_returns_204()
    {
        var result = await Controller().RemoveTag( new PostId(), "dotnet", default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Removing_a_tag_from_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).RemoveTag( new PostId(), "dotnet", default );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── POST /{id}/comments ───────────────────────────────────────────────────

    [Fact]
    public async Task Adding_a_comment_returns_204()
    {
        var result = await Controller().AddComment(
            new PostId(),
            new AddCommentRequestBody { AuthorId = new PersonId(), Content = "Great post!" },
            default
        );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Adding_a_comment_to_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).AddComment(
            new PostId(),
            new AddCommentRequestBody { AuthorId = new PersonId(), Content = "Great post!" },
            default
        );
        Assert.IsType< NotFoundResult >( result );
    }

    [Fact]
    public async Task Adding_a_comment_dispatches_the_correct_command()
    {
        var postId   = new PostId();
        var authorId = new PersonId();
        var mediator = new FakeMediator();
        await Controller( mediator ).AddComment(
            postId,
            new AddCommentRequestBody { AuthorId = authorId, Content = "Great post!" },
            default
        );

        var command = Assert.IsType< AddCommentCommand >( mediator.CapturedRequest );
        Assert.Equal( postId,   command.PostId );
        Assert.Equal( authorId, command.AuthorId );
        Assert.Equal( "Great post!", command.Content );
    }

    // ── DELETE /{id}/comments/{comment-id} ────────────────────────────────────

    [Fact]
    public async Task Deleting_a_comment_returns_204()
    {
        var result = await Controller().DeleteComment( new PostId(), new CommentId(), default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Deleting_a_comment_from_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).DeleteComment( new PostId(), new CommentId(), default );
        Assert.IsType< NotFoundResult >( result );
    }

    [Fact]
    public async Task Deleting_a_comment_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Comment >() );
        var result = await Controller( mediator ).DeleteComment( new PostId(), new CommentId(), default );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── DELETE /{id} ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_post_returns_204()
    {
        var result = await Controller().DeletePost( new PostId(), default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Deleting_a_post_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Post >() );
        var result = await Controller( mediator ).DeletePost( new PostId(), default );
        Assert.IsType< NotFoundResult >( result );
    }
}
