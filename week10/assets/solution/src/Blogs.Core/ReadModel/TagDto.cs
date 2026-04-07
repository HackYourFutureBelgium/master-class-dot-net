namespace Blogs.Core.ReadModel;

public class TagDto
{
    public string Value { get; set; }

    public static explicit operator TagDto( string value ) => new() { Value = value };
}
