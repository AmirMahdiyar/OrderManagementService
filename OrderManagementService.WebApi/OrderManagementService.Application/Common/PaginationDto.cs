namespace OrderManagementService.Application.Common
{
    public record PaginationDto<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int CurrentPage,
        int TotalPages,
        bool HasNextPage,
        bool HasPreviousPage
    );
}
