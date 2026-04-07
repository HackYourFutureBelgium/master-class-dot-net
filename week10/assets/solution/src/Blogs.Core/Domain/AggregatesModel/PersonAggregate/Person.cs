namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

public class Person : Entity< PersonId >, IAggregateRoot
{
    private string _fullName = null!;
    private string? _shortName;
    private string _emailAddress = null!;

    public string FullName
    {
        get => _fullName;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PersonException( PersonExceptionCode.FullNameNullOrWhiteSpace );

            if ( value.Length > 1024 )
                throw new PersonException( PersonExceptionCode.FullNameTooLong );

            _fullName = value;
        }
    }

    public string? ShortName
    {
        get => _shortName;
        set
        {
            if ( value != null && value.Length > 64 )
                throw new PersonException( PersonExceptionCode.ShortNameTooLong );

            _shortName = value;
        }
    }

    public string EmailAddress
    {
        get => _emailAddress;
        set
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PersonException( PersonExceptionCode.EmailAddressNullOrWhiteSpace );

            if ( value.Length > 320 )
                throw new PersonException( PersonExceptionCode.EmailAddressTooLong );

            _emailAddress = value;
        }
    }

    public Person( string fullName, string emailAddress )
    {
        FullName = fullName;
        EmailAddress = emailAddress;
        AddDomainEvent( new PersonRegistered( Id, FullName, EmailAddress ) );
    }
}
