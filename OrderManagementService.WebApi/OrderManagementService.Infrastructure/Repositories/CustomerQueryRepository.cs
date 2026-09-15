using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class CustomerQueryRepository : ICustomerQueryRepository
    {
        private readonly DbSet<Customer> _customers;

        public CustomerQueryRepository(OrderManagementDbContext context)
        {
            _customers = context.Set<Customer>();
        }
    }
}
