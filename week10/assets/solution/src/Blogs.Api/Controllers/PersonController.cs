namespace Blogs.Api.Controllers;

/// <summary>
///
/// </summary>
/// <param name="logger"></param>
/// <param name="mediator"></param>
/// <param name="personQueries"></param>
[ ApiController ]
[ Route( "[controller]" ) ]
[ Produces( MediaTypeNames.Application.Json ) ]
public class PersonController(
    ILogger< PersonController > logger,
    IMediator mediator,
    IPersonQueries personQueries
) : Controller
{
    private readonly ILogger< PersonController > _logger = logger
                                                        ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IMediator _mediator = mediator
                                        ?? throw new ArgumentNullException( nameof( mediator ) );
    private readonly IPersonQueries _personQueries = personQueries
                                                  ?? throw new ArgumentNullException( nameof( personQueries ) );

    /// <summary>
    /// Retrieves a paginated list of people based on the specified search terms.
    /// </summary>
    /// <param name="pageSize">The number of people to return per page.</param>
    /// <param name="pageIndex">The page index to return.</param>
    /// <param name="searchTerm">The search terms to filter the people by.</param>
    /// <param name="sortDirection">The direction to sort the people in.</param>
    /// <param name="sortColumn">The property name (column) to sort the people by.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A paged result of people, containing the people's details, total count, and pagination information.
    /// </returns>
    [ HttpGet ]
    [ ProducesResponseType( typeof( PagedResult< PersonDto > ), StatusCodes.Status200OK ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > FindPeople(
        [ FromQuery( Name = "search-term" ) ] string? searchTerm = null,
        [ FromQuery( Name = "page-index" ), Required, Range( 1, uint.MaxValue ) ] uint pageIndex = 1,
        [ FromQuery( Name = "page-size" ), Required, Range( 1, uint.MaxValue ) ] uint pageSize = 10,
        [ FromQuery( Name = "sort-column" ), Required ] string sortColumn = "fullName",
        [ FromQuery( Name = "sort-direction" ), Required ] SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default
    )
    {
        var (people, total) = await _personQueries.FindPeopleAsync(
            searchTerm,
            pageIndex,
            pageSize,
            sortColumn,
            sortDirection,
            cancellationToken
        );
        var pagedResult = new PagedResult< PersonDto >(
            people,
            total,
            new Pagination( pageIndex, pageSize, sortColumn, sortDirection )
        );
        return Ok( pagedResult );
    }

    /// <summary>
    /// Retrieves a person by their ID.
    /// </summary>
    /// <param name="id">The ID of the person to retrieve.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// The person with the specified ID, or a 404 status code if the person could not be found, or a 400 status code if
    /// the request is invalid.
    /// </returns>
    [ HttpGet( "{id}" ) ]
    [ ProducesResponseType( typeof( PagedResult< PersonDto > ), StatusCodes.Status200OK ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > GetPerson(
        [ FromRoute ] PersonId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok( await _personQueries.GetPersonAsync( id, cancellationToken ) );
        }
        catch ( EntityNotFoundException< Person > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Registers a new person.
    /// </summary>
    /// <param name="body">The request containing the details of the person to register.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 201 status code and the details of the newly registered person, or a 400 status code if the request was
    /// invalid.
    /// </returns>
    [ HttpPost ]
    [ Consumes( MediaTypeNames.Application.Json ) ]
    [ ProducesResponseType( typeof( PersonDto ), StatusCodes.Status201Created ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > RegisterPerson(
        [ FromBody ] RegisterPersonRequestBody body,
        CancellationToken cancellationToken = default
    )
    {
        var newPerson = await _mediator.Send(
            new RegisterPersonCommand( body.FullName, body.EmailAddress ),
            cancellationToken
        );
        return CreatedAtAction( nameof( GetPerson ), new { id = newPerson.Id }, newPerson );
    }

    /// <summary>
    /// Updates a person by their ID.
    /// </summary>
    /// <param name="id">The ID of the person to update.</param>
    /// <param name="body">The request containing the new details of the person.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the person was successfully updated, or a 400 status code if the request was invalid, or a
    /// 404 status code if the person could not be found.
    /// </returns>
    [ HttpPatch( "{id}" ) ]
    [ Consumes( MediaTypeNames.Application.JsonPatch ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > UpdatePerson(
        [ FromRoute ] PersonId id,
        [ FromBody ] JsonPatchDocument< PersonDto > body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new UpdatePersonCommand( id, body ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Person > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a person by their ID.
    /// </summary>
    /// <param name="id">The ID of the person to delete.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the person was successfully deleted, or a 404 status code if the person could not be found,
    /// or a 400 status code if the request was invalid.
    /// </returns>
    [ HttpDelete( "{id}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > DeletePerson(
        [ FromRoute ] PersonId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new DeletePersonCommand( id ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Person > )
        {
            return NotFound();
        }
    }
}
