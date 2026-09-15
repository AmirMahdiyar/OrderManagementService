using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class CustomerCommandRepository : ICustomerCommandRepository
    {
        private readonly DbSet<Customer> _customers;

        public CustomerCommandRepository(OrderManagementDbContext context)
        {
            _customers = context.Set<Customer>();
        }
        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await _customers.AnyAsync(c => c.Id == id, ct);
        public async Task<bool> ExistsAsync(List<Guid> ids, CancellationToken ct = default)
        {
            var distinctIds = ids.Distinct().ToList();
            var existingCount = await _customers.CountAsync(x => distinctIds.Contains(x.Id), ct);
            return existingCount == distinctIds.Count;
        }
    }
}
