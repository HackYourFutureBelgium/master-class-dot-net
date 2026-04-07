namespace Blogs.Api.Controllers;

/// <summary>
///
/// </summary>
/// <param name="logger"></param>
/// <param name="mediator"></param>
/// <param name="blogQueries"></param>
[ ApiController ]
[ Route( "[controller]" ) ]
[ Produces( MediaTypeNames.Application.Json ) ]
public class BlogController(
    ILogger< BlogController > logger,
    IMediator mediator,
    IBlogQueries blogQueries
) : Controller
{
    private readonly ILogger< BlogController > _logger = logger
                                                      ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IMediator _mediator = mediator
                                        ?? throw new ArgumentNullException( nameof( mediator ) );
    private readonly IBlogQueries _blogQueries = blogQueries
                                              ?? throw new ArgumentNullException( nameof( blogQueries ) );

    /// <summary>
    /// Retrieves a blog by its ID.
    /// </summary>
    /// <param name="id">The ID of the blog to retrieve.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// The blog with the specified ID, or a 404 status code if the blog could not be found, or a 400 status code if the
    /// request is invalid.
    /// </returns>
    [ HttpGet( "{id}" ) ]
    [ ProducesResponseType( typeof( BlogDto ), StatusCodes.Status200OK ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > GetBlog(
        [ FromRoute ] BlogId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok( await _blogQueries.GetBlogAsync( id, cancellationToken ) );
        }
        catch ( EntityNotFoundException< Blog > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new blog.
    /// </summary>
    /// <param name="body">The request containing the details of the blog to create.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 201 status code and the details of the newly created blog, or a 400 status code if the request was invalid.
    /// </returns>
    [ HttpPost ]
    [ Consumes( MediaTypeNames.Application.Json ) ]
    [ ProducesResponseType( typeof( BlogDto ), StatusCodes.Status201Created ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > CreateBlog(
        [ FromBody ] CreateBlogRequestBody body,
        CancellationToken cancellationToken = default
    )
    {
        var newBlog = await _mediator.Send(
            new CreateBlogCommand( body.Title, body.OwnerId, body.Description ),
            cancellationToken
        );
        return CreatedAtAction( nameof( GetBlog ), new { id = newBlog.Id }, newBlog );
    }

    /// <summary>
    /// Updates a blog by its ID.
    /// </summary>
    /// <param name="id">The ID of the blog to update.</param>
    /// <param name="body">A JSON patch document containing the changes to apply to the blog.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the blog was successfully updated, or a 400 status code if the request was invalid, or a
    /// 404 status code if the blog could not be found.
    /// </returns>
    [ HttpPatch( "{id}" ) ]
    [ Consumes( MediaTypeNames.Application.JsonPatch ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > UpdateBlog(
        [ FromRoute ] BlogId id,
        [ FromBody ] JsonPatchDocument< Blog > body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new UpdateBlogCommand( id, body ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Blog > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a blog by its ID.
    /// </summary>
    /// <param name="id">The ID of the blog to delete.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the blog was successfully deleted, or a 404 status code if the blog could not be found, or
    /// a 400 status code if the request was invalid.
    /// </returns>
    [ HttpDelete( "{id}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > DeleteBlog(
        [ FromRoute ] BlogId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new DeleteBlogCommand( id ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Blog > )
        {
            return NotFound();
        }
    }
}
