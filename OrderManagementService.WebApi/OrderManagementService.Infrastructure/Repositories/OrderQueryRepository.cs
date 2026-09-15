using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure.Conversions;
using OrderManagementService.Infrastructure.Extensions;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class OrderQueryRepository : IOrderQueryRepository
    {
        private readonly DbSet<Order> _orders;

        public OrderQueryRepository(OrderManagementDbContext context)
        {
            _orders = context.Set<Order>();
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _orders.AsNoTracking()
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<PaginationDto<Order>> GetFilteredOrdersAsync(
            PaginationQuery paginationQuery,
            Guid? customerId,
            string? orderStatus = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken ct = default)
        {
            var query = _orders.AsNoTracking().Include(o => o.Items).AsQueryable();

            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(orderStatus))
            {
                var targetState = OrderStateConverter.ConvertToState(orderStatus);
                query = query.Where(o => o.State == targetState);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedDate <= endDate.Value);
            }

            return await query.CreatePagination(paginationQuery).PaginateAsync(ct);
        }
    }
}
