namespace Blogs.Core.Domain.AggregatesModel.PostAggregate;

public class Tag : ValueObject
{
    private readonly string _value = null!;

    public string Value
    {
        get => _value;
        private init
        {
            if ( string.IsNullOrWhiteSpace( value ) )
                throw new PostException( PostExceptionCode.TagNullOrWhiteSpace );

            if ( value.Length > 256 )
                throw new PostException( PostExceptionCode.TagTooLong );

            _value = value;
        }
    }

    public Tag( string value ) => Value = value;

    protected override IEnumerable< object > GetAtomicValues()
    {
        yield return Value;
    }
}
