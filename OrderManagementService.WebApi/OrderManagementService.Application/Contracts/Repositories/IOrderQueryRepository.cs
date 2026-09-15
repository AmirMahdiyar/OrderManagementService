using OrderManagementService.Application.Common;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts.Repositories
{
    public interface IOrderQueryRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<PaginationDto<Order>> GetFilteredOrdersAsync(
            PaginationQuery paginationQuery,
            Guid? customerId,
            string? orderStatus = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken ct = default);
    }
}
