using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.Services
{
    public interface IOrderInventoryChecker
    {
        Task<bool> HasSufficientStockAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
    }
}
