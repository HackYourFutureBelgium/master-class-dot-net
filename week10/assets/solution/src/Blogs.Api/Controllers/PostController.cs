namespace Blogs.Api.Controllers;

/// <summary>
/// Controller for managing blog posts.
/// </summary>
/// <param name="logger"></param>
/// <param name="mediator"></param>
/// <param name="postQueries"></param>
[ ApiController ]
[ Route( "[controller]" ) ]
[ Produces( MediaTypeNames.Application.Json ) ]
public class PostController(
    ILogger< PostController > logger,
    IMediator mediator,
    IPostQueries postQueries
) : Controller
{
    private readonly ILogger< PostController > _logger = logger
                                                      ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IMediator _mediator = mediator
                                        ?? throw new ArgumentNullException( nameof( mediator ) );
    private readonly IPostQueries _postQueries = postQueries
                                              ?? throw new ArgumentNullException( nameof( postQueries ) );

    /// <summary>
    /// Retrieves a paginated list of posts.
    /// </summary>
    /// <param name="pageSize">The number of posts to return per page.</param>
    /// <param name="tags">The tags to filter the posts by.</param>
    /// <param name="pageIndex">The page index to return.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <param name="searchTerm">The term to search for in the posts' content or title.</param>
    /// <returns>
    /// A paged result of posts, containing the posts' details, total count, and pagination information.
    /// </returns>
    [ HttpGet ]
    [ ProducesResponseType( typeof( PagedResult< PostDto > ), StatusCodes.Status200OK ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > FindPosts(
        [ FromQuery( Name = "search-term" ) ] string? searchTerm = null,
        [ FromQuery( Name = "tags" ) ] IEnumerable< string >? tags = null,
        [ FromQuery( Name = "page-index" ), Required, Range( 1, uint.MaxValue ) ] uint pageIndex = 1,
        [ FromQuery( Name = "page-size" ), Required, Range( 1, uint.MaxValue ) ] uint pageSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var (posts, total) = await _postQueries.FindPostsAsync(
            searchTerm,
            tags,
            pageIndex,
            pageSize,
            cancellationToken
        );
        var pagedResult = new PagedResult< PostDto >( posts, total, new Pagination( pageIndex, pageSize ) );
        return Ok( pagedResult );
    }

    /// <summary>
    /// Retrieves a post by its ID.
    /// </summary>
    /// <param name="id">The ID of the post to retrieve.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// The post with the specified ID, or a 404 status code if the post could not be found, or a 400 status code if the
    /// request is invalid.
    /// </returns>
    [ HttpGet( "{id}" ) ]
    [ ProducesResponseType( typeof( PostDto ), StatusCodes.Status200OK ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > GetPost(
        [ FromRoute ] PostId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return Ok( await _postQueries.GetPostAsync( id, cancellationToken ) );
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="body">The request containing the details of the post to create.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 201 status code and the details of the newly created post, or a 400 status code if the request was invalid.
    /// </returns>
    [ HttpPost ]
    [ Consumes( MediaTypeNames.Application.Json ) ]
    [ ProducesResponseType( typeof( PostDto ), StatusCodes.Status201Created ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > CreatePost(
        [ FromBody ] CreatePostRequestBody body,
        CancellationToken cancellationToken = default
    )
    {
        var newPost = await _mediator.Send(
            new CreatePostCommand( body.BlogId, body.Title, body.Content ),
            cancellationToken
        );
        return CreatedAtAction( nameof( GetPost ), new { id = newPost.Id }, newPost );
    }

    /// <summary>
    /// Updates a post by its ID.
    /// </summary>
    /// <param name="id">The ID of the post to update.</param>
    /// <param name="body">The request containing the new details of the person.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the post was successfully updated, or a 400 status code if the request was invalid, or a
    /// 404 status code if the post could not be found.
    /// </returns>
    [ HttpPatch( "{id}" ) ]
    [ Consumes( MediaTypeNames.Application.JsonPatch ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > UpdatePost(
        [ FromRoute ] PostId id,
        [ FromBody ] JsonPatchDocument< Post > body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new UpdatePostCommand( id, body ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Adds a tag to a post.
    /// </summary>
    /// <param name="id">The ID of the post to add the tag to.</param>
    /// <param name="tag">The tag to add to the post.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 200 status code if the tag was successfully added, or a 404 status code if the post could not be found, or a
    /// 400 status code if the request was invalid.
    /// </returns>
    [ HttpPost( "{id}/tags/{tag}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > AddTag(
        [ FromRoute ] PostId id,
        [ FromRoute ] string tag,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new AddTagCommand( id, tag ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Adds a comment to a post.
    /// </summary>
    /// <param name="id">The ID of the post to add the comment to.</param>
    /// <param name="body">The request containing the details of the comment to add.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 200 status code if the comment was successfully added, or a 404 status code if the post could not be found, or
    /// a 400 status code if the request was invalid.
    /// </returns>
    [ HttpPost( "{id}/comments" ) ]
    [ Consumes( MediaTypeNames.Application.Json ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > AddComment(
        [ FromRoute ] PostId id,
        [ FromBody ] AddCommentRequestBody body,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new AddCommentCommand( id, body.AuthorId, body.Content ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a post by its ID.
    /// </summary>
    /// <param name="id">The ID of the post to delete.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the post was successfully deleted, or a 404 status code if the post could not be found, or
    /// a 400 status code if the request was invalid.
    /// </returns>
    [ HttpDelete( "{id}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > DeletePost(
        [ FromRoute ] PostId id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new DeletePostCommand( id ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a tag from a post.
    /// </summary>
    /// <param name="id">The ID of the post that the tag belongs to.</param>
    /// <param name="tag">The tag to delete.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the tag was successfully deleted, or a 404 status code if the tag or the post could not be
    /// found, or a 400 status code if the request was invalid.
    /// </returns>
    [ HttpDelete( "{id}/tags/{tag}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > RemoveTag(
        [ FromRoute ] PostId id,
        [ FromRoute( Name = "tag" ) ] string tag,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new DeleteTagCommand( id, tag ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a comment by its ID.
    /// </summary>
    /// <param name="id">The ID of the post that the comment belongs to.</param>
    /// <param name="commentId">The ID of the comment to delete.</param>
    /// <param name="cancellationToken">A token that allows the operation to be cancelled.</param>
    /// <returns>
    /// A 204 status code if the comment was successfully deleted, or a 404 status code if the comment or the post could
    /// not be found, or a 400 status code if the request was invalid.
    /// </returns>
    [ HttpDelete( "{id}/comments/{comment-id}" ) ]
    [ ProducesResponseType( StatusCodes.Status204NoContent ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain ) ]
    [ ProducesResponseType( typeof( string ), StatusCodes.Status500InternalServerError, MediaTypeNames.Text.Plain ) ]
    public async Task< IActionResult > DeleteComment(
        [ FromRoute ] PostId id,
        [ FromRoute( Name = "comment-id" ) ] CommentId commentId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await _mediator.Send( new DeleteCommentCommand( id, commentId ), cancellationToken );
            return NoContent();
        }
        catch ( EntityNotFoundException< Post > )
        {
            return NotFound();
        }
        catch ( EntityNotFoundException< Comment > )
        {
            return NotFound();
        }
    }
}
