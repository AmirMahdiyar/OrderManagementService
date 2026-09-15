using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts.Repositories
{
    public interface IProductCommandRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(List<Guid> ids, CancellationToken ct = default);
        Task<List<Product>> GetProductsByIdsAsync(List<Guid> ids, CancellationToken ct = default);
    }
}
