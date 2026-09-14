using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
