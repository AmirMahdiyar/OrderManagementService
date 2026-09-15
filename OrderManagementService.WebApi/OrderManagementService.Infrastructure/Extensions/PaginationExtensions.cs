using OrderManagementService.Application.Common;
using OrderManagementService.Infrastructure.Helper;

namespace OrderManagementService.Infrastructure.Extensions
{
    public static class PaginationExtensions
    {
        public static Pagination<T> CreatePagination<T>(this IQueryable<T> primitiveValues, PaginationQuery paginationInfo)
            => new Pagination<T>(primitiveValues, paginationInfo);
    }
}
