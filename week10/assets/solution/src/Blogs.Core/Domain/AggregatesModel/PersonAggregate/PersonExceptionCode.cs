namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

public class PersonExceptionCode : Enumeration
{
    public static readonly PersonExceptionCode FullNameNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000001" ), "FULL_NAME_NULL_OR_WHITE_SPACE");
    public static readonly PersonExceptionCode FullNameTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000002" ), "FULL_NAME_TOO_LONG");
    public static readonly PersonExceptionCode ShortNameTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000003" ), "SHORT_NAME_TOO_LONG");
    public static readonly PersonExceptionCode EmailAddressNullOrWhiteSpace = new( Guid.Parse( "00000000-0000-0000-0000-000000000004" ), "EMAIL_ADDRESS_NULL_OR_WHITE_SPACE");
    public static readonly PersonExceptionCode EmailAddressTooLong = new( Guid.Parse( "00000000-0000-0000-0000-000000000005" ), "EMAIL_ADDRESS_TOO_LONG");

    private PersonExceptionCode( Guid id, string name )
        : base( id, name ) { }
}
