namespace Blogs.Core.ReadModel;

[ ExcludeFromCodeCoverage ]
public class PersonDto
{
    public Guid Id { get; set; }
    public string EmailAddress { get; set; }
    public string FullName { get; set; }
    public string? ShortName { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModifiedOn { get; set; }

    public static explicit operator PersonDto( Person person )
        => new()
        {
            Id = person.Id.Value,
            EmailAddress = person.EmailAddress,
            FullName = person.FullName,
            ShortName = person.ShortName,
            CreatedOn = person.CreatedOn,
            LastModifiedOn = person.LastModifiedOn
        };
}
