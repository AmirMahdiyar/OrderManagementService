using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Services;

namespace OrderManagementService.Infrastructure.Services
{
    public class OrderInventoryChecker : IOrderInventoryChecker
    {
        private readonly OrderManagementDbContext _context;

        public OrderInventoryChecker(OrderManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasSufficientStockAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
        {
            var productIds = items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return false;

                if (product.Stock.Value < item.Quantity.Value)
                    return false;
            }

            return true;
        }
    }
}
