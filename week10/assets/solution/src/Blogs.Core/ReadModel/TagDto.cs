namespace Blogs.Core.ReadModel;

[ ExcludeFromCodeCoverage ]
public class TagDto
{
    public string Value { get; set; }

    public static explicit operator TagDto( string value ) => new() { Value = value };
}
