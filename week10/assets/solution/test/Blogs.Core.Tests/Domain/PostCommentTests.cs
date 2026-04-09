namespace Blogs.Core.Tests.Domain;

public class PostCommentTests
{
    private static Post APost() => new( new BlogId(), "My title", "Some content." );

    [Fact]
    public void Adding_a_comment_appends_it_to_the_post()
    {
        var post = APost();
        post.AddComment( new PersonId(), "Great post!" );
        Assert.Single( post.Comments );
    }

    [Fact]
    public void Adding_a_comment_raises_a_CommentAdded_event()
    {
        var post = APost();
        var comment = post.AddComment( new PersonId(), "Great post!" );

        var evt = Assert.Single( comment.DomainEvents.OfType<CommentAdded>() );
        Assert.Equal( post.Id, evt.PostId );
    }

    [Fact]
    public void A_comment_records_its_author()
    {
        var authorId = new PersonId();
        var post = APost();
        post.AddComment( authorId, "Great post!" );

        Assert.Equal( authorId, post.Comments.Single().AuthorId );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void A_comment_requires_content( string blankContent )
    {
        var post = APost();
        var exception = Assert.Throws<PostException>( () => post.AddComment( new PersonId(), blankContent ) );
        Assert.Equal( PostExceptionCode.CommentTextNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Comment_content_cannot_exceed_1024_characters()
    {
        var post = APost();
        var tooLong = new string( 'a', 1025 );
        var exception = Assert.Throws<PostException>( () => post.AddComment( new PersonId(), tooLong ) );
        Assert.Equal( PostExceptionCode.CommentTextTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Deleting_a_comment_removes_it_from_the_post()
    {
        var post = APost();
        var comment = post.AddComment( new PersonId(), "Great post!" );
        post.DeleteComment( comment );
        Assert.Empty( post.Comments );
    }
}
