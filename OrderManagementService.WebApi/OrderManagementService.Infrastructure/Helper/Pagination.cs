using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Common;
using System.Linq.Expressions;

namespace OrderManagementService.Infrastructure.Helper
{
    public class Pagination<T>
    {
        private IQueryable<T> _items;
        private readonly PaginationQuery _paginationInfo;

        public Pagination(IQueryable<T> primitiveItems, PaginationQuery paginationInfo)
        {
            _items = primitiveItems;
            _paginationInfo = paginationInfo;
        }

        public void Filter(Expression<Func<T, bool>> predicate)
        {
            _items = _items.Where(predicate);
        }

        public async Task<PaginationDto<T>> PaginateAsync(CancellationToken cancellationToken = default)
        {
            var totalCount = await _items.CountAsync(cancellationToken);

            var items = await _items
                .Skip((_paginationInfo.Page - 1) * _paginationInfo.Size)
                .Take(_paginationInfo.Size)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalCount / (double)_paginationInfo.Size);

            return new PaginationDto<T>(
                Items: items,
                TotalCount: totalCount,
                CurrentPage: _paginationInfo.Page,
                TotalPages: totalPages,
                HasNextPage: _paginationInfo.Page < totalPages,
                HasPreviousPage: _paginationInfo.Page > 1
            );
        }
    }
}
