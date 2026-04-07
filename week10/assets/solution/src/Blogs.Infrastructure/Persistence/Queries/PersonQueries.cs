namespace Blogs.Infrastructure.Persistence.Queries;

[ ExcludeFromCodeCoverage ]
public class PersonQueries( ILogger< PersonQueries > logger, IConnectionFactory connectionFactory ) : IPersonQueries
{
    private readonly ILogger< PersonQueries > _logger = logger
                                                     ?? throw new ArgumentNullException( nameof( logger ) );
    private readonly IConnectionFactory _connectionFactory = connectionFactory
                                                          ?? throw new ArgumentNullException( nameof( connectionFactory ) );

    public async Task< (IEnumerable< PersonDto >, uint) > FindPeopleAsync(
        string? searchTerm,
        uint pageIndex = 1,
        uint pageSize = 10,
        string sortColumn = "fullName",
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default
    )
    {
        var query = """
                    SELECT COUNT(*)
                      FROM Person p
                     WHERE p.DeletedOn IS NULL
                       AND (@SearchTerm IS NULL
                           OR ( (p.EmailAddress COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI)
                             OR (p.FullName     COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI)
                             OR (p.ShortName    COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI))
                           );
                    SELECT p.Id
                          ,p.EmailAddress
                          ,p.FullName
                          ,p.ShortName
                          ,p.CreatedOn
                          ,p.LastModifiedOn
                      FROM Person p
                     WHERE p.DeletedOn IS NULL
                       AND (@SearchTerm IS NULL
                           OR ( (p.EmailAddress COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI)
                             OR (p.FullName     COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI)
                             OR (p.ShortName    COLLATE Latin1_general_CI_AI LIKE @LikeSearchTerm COLLATE Latin1_general_CI_AI))
                           )
                     ORDER BY
                       CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'emailAddress'   THEN EmailAddress   END,
                       CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'fullName'       THEN FullName       END,
                       CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'shortName'      THEN ShortName      END,
                       CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'createdOn'      THEN CreatedOn      END,
                       CASE WHEN @SortDirection = 'Ascending'  AND @SortColumn = 'lastModifiedOn' THEN LastModifiedOn END,
                       CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'emailAddress'   THEN EmailAddress   END DESC,
                       CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'fullName'       THEN FullName       END DESC,
                       CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'shortName'      THEN ShortName      END DESC,
                       CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'createdOn'      THEN CreatedOn      END DESC,
                       CASE WHEN @SortDirection = 'Descending' AND @SortColumn = 'lastModifiedOn' THEN LastModifiedOn END DESC
                     OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                    """;

        await using var connection = _connectionFactory.CreateConnection();
        var parameters = new
        {
            SearchTerm = searchTerm,
            LikeSearchTerm = !string.IsNullOrWhiteSpace( searchTerm )
                ? $"%{searchTerm.EncodeForSqlLike()}%"
                : string.Empty,
            SortDirection = sortDirection.ToString(),
            SortColumn = sortColumn,
            Skip = ( long )pageSize * ( pageIndex - 1 ),
            Take = ( long )pageSize
        };
        await using var multi = await connection.QueryMultipleAsync( query, parameters );
        var count = await multi.ReadFirstAsync< uint >();
        var people = await multi.ReadAsync< PersonDto >();
        return ( people, count );
    }

    public async Task< PersonDto > GetPersonAsync( PersonId personId, CancellationToken cancellationToken = default )
    {
        var query = """
                    SELECT p.Id
                          ,p.EmailAddress
                          ,p.FullName
                          ,p.ShortName
                          ,p.CreatedOn
                          ,p.LastModifiedOn
                      FROM Person p
                     WHERE p.DeletedOn IS NULL
                       AND p.Id = @PersonId
                    """;
        await using var connection = _connectionFactory.CreateConnection();
        var parameters = new { PersonId = personId.Value };
        var personDto = await connection.QuerySingleOrDefaultAsync< PersonDto >( query, parameters );

        if ( personDto is null )
            throw new EntityNotFoundException< Person >( nameof( Person.Id ), personId );

        return personDto;
    }
}
