using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts.Repositories
{
    public interface IUserQueryRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    }
}
