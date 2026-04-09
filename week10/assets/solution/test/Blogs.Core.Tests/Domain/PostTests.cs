namespace Blogs.Core.Tests.Domain;

public class PostTests
{
    // ── Creation ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void A_post_requires_a_title( string blankTitle )
    {
        var exception = Assert.Throws<PostException>( () => new Post( new BlogId(), blankTitle, "Some content." ) );
        Assert.Equal( PostExceptionCode.TitleNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Post_title_cannot_exceed_256_characters()
    {
        var tooLong = new string( 'a', 257 );
        var exception = Assert.Throws<PostException>( () => new Post( new BlogId(), tooLong, "Some content." ) );
        Assert.Equal( PostExceptionCode.TitleTooLong, exception.ExceptionCode );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void A_post_requires_content( string blankContent )
    {
        var exception = Assert.Throws<PostException>( () => new Post( new BlogId(), "My title", blankContent ) );
        Assert.Equal( PostExceptionCode.ContentNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Post_content_cannot_exceed_4096_characters()
    {
        var tooLong = new string( 'a', 4097 );
        var exception = Assert.Throws<PostException>( () => new Post( new BlogId(), "My title", tooLong ) );
        Assert.Equal( PostExceptionCode.ContentTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void A_new_post_starts_as_a_draft()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        Assert.Equal( PostStatus.Draft, post.PostStatus );
    }

    // ── Publishing ────────────────────────────────────────────────────────────

    [Fact]
    public void Publishing_a_post_marks_it_as_published()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();
        Assert.Equal( PostStatus.Published, post.PostStatus );
    }

    [Fact]
    public void Publishing_a_post_records_when_it_was_published()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();
        Assert.NotNull( post.PublishedOn );
    }

    [Fact]
    public void Publishing_a_post_raises_a_PostPublished_event()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();

        var evt = Assert.Single( post.DomainEvents.OfType<PostPublished>() );
        Assert.Equal( post.Id, evt.PostId );
    }

    [Fact]
    public void A_post_cannot_be_published_twice()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();

        var exception = Assert.Throws<PostException>( () => post.Publish() );
        Assert.Equal( PostExceptionCode.AlreadyPublished, exception.ExceptionCode );
    }

    // ── Unpublishing ──────────────────────────────────────────────────────────

    [Fact]
    public void Unpublishing_a_post_reverts_it_to_draft()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();
        post.Unpublish();
        Assert.Equal( PostStatus.Draft, post.PostStatus );
    }

    [Fact]
    public void Unpublishing_a_post_clears_the_published_date()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Publish();
        post.Unpublish();
        Assert.Null( post.PublishedOn );
    }

    [Fact]
    public void A_post_that_was_never_published_cannot_be_unpublished()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        var exception = Assert.Throws<PostException>( () => post.Unpublish() );
        Assert.Equal( PostExceptionCode.NotPublished, exception.ExceptionCode );
    }

    // ── Archiving ─────────────────────────────────────────────────────────────

    [Fact]
    public void Archiving_a_post_marks_it_as_archived()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Archive();
        Assert.Equal( PostStatus.Archived, post.PostStatus );
    }

    [Fact]
    public void Archiving_a_post_records_when_it_was_archived()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Archive();
        Assert.NotNull( post.ArchivedOn );
    }

    [Fact]
    public void A_post_cannot_be_archived_twice()
    {
        var post = new Post( new BlogId(), "My title", "Some content." );
        post.Archive();
        var exception = Assert.Throws<PostException>( () => post.Archive() );
        Assert.Equal( PostExceptionCode.AlreadyPublished, exception.ExceptionCode );
    }
}
