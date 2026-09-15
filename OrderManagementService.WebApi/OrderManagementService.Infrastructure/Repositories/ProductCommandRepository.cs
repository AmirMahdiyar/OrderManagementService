using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class ProductCommandRepository : IProductCommandRepository
    {
        private readonly DbSet<Product> _products;

        public ProductCommandRepository(OrderManagementDbContext context)
        {
            _products = context.Set<Product>();
        }
        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await _products.AnyAsync(c => c.Id == id, ct);

        public async Task<bool> ExistsAsync(List<Guid> ids, CancellationToken ct = default)
        {
            var distinctIds = ids.Distinct().ToList();

            var existingCount = await _products.CountAsync(x => distinctIds.Contains(x.Id), ct);

            return existingCount == distinctIds.Count;
        }
        public async Task<List<Product>> GetProductsByIdsAsync(List<Guid> ids, CancellationToken ct = default)
        {
            var distinctIds = ids.Distinct().ToList();
            return await _products.Where(p => distinctIds.Contains(p.Id)).ToListAsync(ct);
        }

    }
}
