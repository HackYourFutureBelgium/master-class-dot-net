namespace Blogs.Application.Tests.Commands;

public class PersonCommandHandlerTests
{
    // ── Register ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task Registering_a_person_persists_a_new_person_entity()
    {
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterPersonCommandHandler( NullLogger< RegisterPersonCommandHandler >.Instance, unitOfWork );

        await handler.Handle( new RegisterPersonCommand( "Alice Smith", "alice@example.com" ), default );

        var person = Assert.IsType< Person >( Assert.Single( unitOfWork.NewEntities ) );
        Assert.Equal( "Alice Smith", person.FullName );
        Assert.Equal( "alice@example.com", person.EmailAddress );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Registering_a_person_returns_a_dto_matching_the_registered_data()
    {
        var handler = new RegisterPersonCommandHandler(
            NullLogger< RegisterPersonCommandHandler >.Instance,
            new FakeUnitOfWork()
        );

        var dto = await handler.Handle( new RegisterPersonCommand( "Alice Smith", "alice@example.com" ), default );

        Assert.Equal( "Alice Smith", dto.FullName );
        Assert.Equal( "alice@example.com", dto.EmailAddress );
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_person_applies_the_patch_and_saves()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new UpdatePersonCommandHandler(
            NullLogger< UpdatePersonCommandHandler >.Instance,
            unitOfWork,
            new FakePersonRepository( person )
        );
        var patch = new JsonPatchDocument< PersonDto >();
        patch.Replace( p => p.FullName, "Alice Updated" );

        await handler.Handle( new UpdatePersonCommand( person.Id, patch ), default );

        Assert.Equal( "Alice Updated", person.FullName );
        Assert.Contains( person, unitOfWork.DirtyEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Updating_a_person_that_does_not_exist_throws()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        var handler = new UpdatePersonCommandHandler(
            NullLogger< UpdatePersonCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePersonRepository( person )
        );
        var patch = new JsonPatchDocument< PersonDto >();

        await Assert.ThrowsAsync< EntityNotFoundException< Person > >(
            () => handler.Handle( new UpdatePersonCommand( new PersonId(), patch ), default )
        );
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_person_marks_them_for_deletion_and_commits()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeletePersonCommandHandler(
            NullLogger< DeletePersonCommandHandler >.Instance,
            unitOfWork,
            new FakePersonRepository( person )
        );

        await handler.Handle( new DeletePersonCommand( person.Id ), default );

        Assert.Contains( person, unitOfWork.DeletedEntities );
        Assert.True( unitOfWork.CommitCalled );
    }

    [Fact]
    public async Task Deleting_a_person_that_does_not_exist_throws()
    {
        var person = new Person( "Alice Smith", "alice@example.com" );
        var handler = new DeletePersonCommandHandler(
            NullLogger< DeletePersonCommandHandler >.Instance,
            new FakeUnitOfWork(),
            new FakePersonRepository( person )
        );

        await Assert.ThrowsAsync< EntityNotFoundException< Person > >(
            () => handler.Handle( new DeletePersonCommand( new PersonId() ), default )
        );
    }
}
