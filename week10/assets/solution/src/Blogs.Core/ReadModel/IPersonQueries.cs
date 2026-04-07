namespace Blogs.Core.ReadModel;

public interface IPersonQueries
{
    Task< (IEnumerable< PersonDto >, uint) > FindPeopleAsync(
        string? searchTerm,
        uint pageIndex = 1,
        uint pageSize = 10,
        string sortColumn = "fullName",
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default
    );

    Task< PersonDto > GetPersonAsync( PersonId personId, CancellationToken cancellationToken = default );
}
