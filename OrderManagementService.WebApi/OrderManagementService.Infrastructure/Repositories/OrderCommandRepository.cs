using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class OrderCommandRepository : IOrderCommandRepository
    {
        private readonly DbSet<Order> _orders;

        public OrderCommandRepository(OrderManagementDbContext context)
        {
            _orders = context.Set<Order>();
        }
        public void Add(Order order)
        {
            _orders.Add(order);
        }
        public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default)
            => await _orders
                        .Include(o => o.Items)
                        .SingleOrDefaultAsync(o => o.Id == id, ct);

        public void AddRange(IEnumerable<Order> orders)
        {
            _orders.AddRange(orders);
        }
        public void Delete(Order order)
        {
            _orders.Remove(order);
        }
    }
}
