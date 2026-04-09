namespace Blogs.Infrastructure.Extensions;

[ ExcludeFromCodeCoverage ]
public static class StringExtensions
{
    public static string EncodeForSqlLike( this string value )
        => value.Replace( "%", "[%]" )
                .Replace( "_", "[_]" );
}
