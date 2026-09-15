using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Infrastructure.Configurations;
using OrderManagementService.Infrastructure.Outbox;

namespace OrderManagementService.Infrastructure
{
    public class OrderManagementDbContext : DbContext
    {
        public OrderManagementDbContext(DbContextOptions<OrderManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<User> Users => Set<User>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
