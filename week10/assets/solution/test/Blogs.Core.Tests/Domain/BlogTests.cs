namespace Blogs.Core.Tests.Domain;

public class BlogTests
{
    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void A_blog_requires_a_title( string blankTitle )
    {
        var exception = Assert.Throws<BlogException>( () => new Blog( blankTitle, new PersonId() ) );
        Assert.Equal( BlogExceptionCode.TitleNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Blog_title_cannot_exceed_256_characters()
    {
        var tooLong = new string( 'a', 257 );
        var exception = Assert.Throws<BlogException>( () => new Blog( tooLong, new PersonId() ) );
        Assert.Equal( BlogExceptionCode.TitleTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Blog_description_cannot_exceed_1024_characters()
    {
        var blog = new Blog( "My Blog", new PersonId() );
        var tooLong = new string( 'a', 1025 );
        var exception = Assert.Throws<BlogException>( () => blog.Description = tooLong );
        Assert.Equal( BlogExceptionCode.DescriptionTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Blog_description_is_optional()
    {
        var blog = new Blog( "My Blog", new PersonId() );
        Assert.Null( blog.Description );
    }

    [Fact]
    public void A_blog_belongs_to_its_owner()
    {
        var ownerId = new PersonId();
        var blog = new Blog( "My Blog", ownerId );
        Assert.Equal( ownerId, blog.OwnerId );
    }
}
