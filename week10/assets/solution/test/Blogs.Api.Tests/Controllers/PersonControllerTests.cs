namespace Blogs.Api.Tests.Controllers;

public class PersonControllerTests
{
    private static PersonDto APersonDto() => new()
    {
        Id = Guid.NewGuid(), FullName = "Alice Smith", EmailAddress = "alice@example.com"
    };

    private static PersonController Controller( FakeMediator? mediator = null, FakePersonQueries? queries = null )
        => new(
            NullLogger< PersonController >.Instance,
            mediator ?? new FakeMediator(),
            queries  ?? new FakePersonQueries()
        );

    // ── GET /{id} ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Getting_a_person_returns_200_with_the_person_dto()
    {
        var dto = APersonDto();
        var result = await Controller( queries: new FakePersonQueries( dto ) )
            .GetPerson( new PersonId(), default );

        var ok = Assert.IsType< OkObjectResult >( result );
        Assert.Same( dto, ok.Value );
    }

    [Fact]
    public async Task Getting_a_person_that_does_not_exist_returns_404()
    {
        var result = await Controller().GetPerson( new PersonId(), default );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── GET / ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Finding_people_returns_200_with_a_paged_result()
    {
        var dto = APersonDto();
        var result = await Controller( queries: new FakePersonQueries( dto ) )
            .FindPeople( cancellationToken: default );

        var ok = Assert.IsType< OkObjectResult >( result );
        var paged = Assert.IsType< PagedResult< PersonDto > >( ok.Value );
        Assert.Single( paged.Items );
    }

    // ── POST / ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Registering_a_person_returns_201_pointing_to_the_new_person()
    {
        var dto = APersonDto();
        var mediator = new FakeMediator( dto );
        var result = await Controller( mediator ).RegisterPerson(
            new RegisterPersonRequestBody { FullName = "Alice Smith", EmailAddress = "alice@example.com" },
            default
        );

        var created = Assert.IsType< CreatedAtActionResult >( result );
        Assert.Equal( nameof( PersonController.GetPerson ), created.ActionName );
        Assert.Same( dto, created.Value );
    }

    [Fact]
    public async Task Registering_a_person_dispatches_the_correct_command()
    {
        var mediator = new FakeMediator( APersonDto() );
        await Controller( mediator ).RegisterPerson(
            new RegisterPersonRequestBody { FullName = "Alice Smith", EmailAddress = "alice@example.com" },
            default
        );

        var command = Assert.IsType< RegisterPersonCommand >( mediator.CapturedRequest );
        Assert.Equal( "Alice Smith", command.Fullname );
        Assert.Equal( "alice@example.com", command.EmailAddress );
    }

    // ── PATCH /{id} ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Updating_a_person_returns_204()
    {
        var result = await Controller().UpdatePerson(
            new PersonId(), new JsonPatchDocument< PersonDto >(), default
        );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Updating_a_person_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Person >() );
        var result = await Controller( mediator ).UpdatePerson(
            new PersonId(), new JsonPatchDocument< PersonDto >(), default
        );
        Assert.IsType< NotFoundResult >( result );
    }

    // ── DELETE /{id} ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Deleting_a_person_returns_204()
    {
        var result = await Controller().DeletePerson( new PersonId(), default );
        Assert.IsType< NoContentResult >( result );
    }

    [Fact]
    public async Task Deleting_a_person_that_does_not_exist_returns_404()
    {
        var mediator = new FakeMediator();
        mediator.ThrowOnNextSend( new EntityNotFoundException< Person >() );
        var result = await Controller( mediator ).DeletePerson( new PersonId(), default );
        Assert.IsType< NotFoundResult >( result );
    }
}
