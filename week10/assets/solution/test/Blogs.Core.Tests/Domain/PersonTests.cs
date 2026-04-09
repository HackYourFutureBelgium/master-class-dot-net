namespace Blogs.Core.Tests.Domain;

public class PersonTests
{
    [Fact]
    public void Registering_a_person_requires_a_full_name()
    {
        var exception = Assert.Throws<PersonException>( () => new Person( "", "alice@example.com" ) );
        Assert.Equal( PersonExceptionCode.FullNameNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Full_name_cannot_be_blank( string blankName )
    {
        var exception = Assert.Throws<PersonException>( () => new Person( blankName, "alice@example.com" ) );
        Assert.Equal( PersonExceptionCode.FullNameNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Full_name_cannot_exceed_1024_characters()
    {
        var tooLong = new string( 'a', 1025 );
        var exception = Assert.Throws<PersonException>( () => new Person( tooLong, "alice@example.com" ) );
        Assert.Equal( PersonExceptionCode.FullNameTooLong, exception.ExceptionCode );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "   " )]
    public void Email_address_cannot_be_blank( string blankEmail )
    {
        var exception = Assert.Throws<PersonException>( () => new Person( "Alice Smith", blankEmail ) );
        Assert.Equal( PersonExceptionCode.EmailAddressNullOrWhiteSpace, exception.ExceptionCode );
    }

    [Fact]
    public void Email_address_cannot_exceed_320_characters()
    {
        var tooLong = new string( 'a', 321 );
        var exception = Assert.Throws<PersonException>( () => new Person( "Alice Smith", tooLong ) );
        Assert.Equal( PersonExceptionCode.EmailAddressTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Short_name_cannot_exceed_64_characters()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        var tooLong = new string( 'a', 65 );
        var exception = Assert.Throws<PersonException>( () => person.ShortName = tooLong );
        Assert.Equal( PersonExceptionCode.ShortNameTooLong, exception.ExceptionCode );
    }

    [Fact]
    public void Short_name_is_optional()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        Assert.Null( person.ShortName );
    }

    [Fact]
    public void Registering_a_person_raises_a_PersonRegistered_event()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );

        var evt = Assert.Single( person.DomainEvents.OfType<PersonRegistered>() );
        Assert.Equal( person.Id, evt.PersonId );
        Assert.Equal( "Alice Smith", evt.FullName );
        Assert.Equal( "alice@example.com", evt.EmailAddress );
    }
}
