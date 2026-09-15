using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Infrastructure.Repositories
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly DbSet<User> _users;

        public UserQueryRepository(OrderManagementDbContext context)
        {
            _users = context.Set<User>();
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => await _users.SingleOrDefaultAsync(u => u.Username == username, ct);
    }
}
