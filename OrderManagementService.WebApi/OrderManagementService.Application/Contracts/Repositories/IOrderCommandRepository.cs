using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts.Repositories
{
    public interface IOrderCommandRepository
    {
        void Add(Order order);
        void AddRange(IEnumerable<Order> orders);
        void Delete(Order order);
        Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    }
}

