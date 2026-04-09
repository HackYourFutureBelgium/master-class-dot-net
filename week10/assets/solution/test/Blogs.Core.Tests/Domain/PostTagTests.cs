namespace Blogs.Core.Tests.Domain;

public class PostTagTests
{
    private static Post APost() => new( new BlogId(), "My title", "Some content." );

    [Fact]
    public void Adding_a_tag_appends_it_to_the_post()
    {
        var post = APost();
        post.AddTag( "dotnet" );
        Assert.Single( post.Tags );
    }

    [Fact]
    public void Adding_the_same_tag_twice_does_not_create_a_duplicate()
    {
        var post = APost();
        var tag = post.AddTag( "dotnet" );
        post.AddTag( tag );
        Assert.Single( post.Tags );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void A_tag_requires_a_value( string blankTag )
    {
        var post = APost();
        var exception = Assert.Throws<PostException>( () => post.AddTag( blankTag ) );
        Assert.Equal( PostExceptionCode.TagNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Tag_value_cannot_exceed_256_characters()
    {
        var post = APost();
        var tooLong = new string( 'a', 257 );
        var exception = Assert.Throws<PostException>( () => post.AddTag( tooLong ) );
        Assert.Equal( PostExceptionCode.TagTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Deleting_a_tag_removes_it_from_the_post()
    {
        var post = APost();
        var tag = post.AddTag( "dotnet" );
        post.DeleteTag( tag );
        Assert.Empty( post.Tags );
    }

    [Fact]
    public void Deleting_a_tag_that_is_not_on_the_post_is_a_no_op()
    {
        var post = APost();
        post.AddTag( "dotnet" );
        var otherTag = new Tag( "csharp" );
        post.DeleteTag( otherTag );
        Assert.Single( post.Tags );
    }
}
