namespace Blogs.Core.Domain.AggregatesModel.PersonAggregate;

public class PersonRegistered( PersonId personId, string fullName, string emailAddress ) : DomainEvent
{
    public PersonId PersonId { get; } = personId;
    public string FullName { get; set; } = fullName;
    public string EmailAddress { get; set; } = emailAddress;
}
