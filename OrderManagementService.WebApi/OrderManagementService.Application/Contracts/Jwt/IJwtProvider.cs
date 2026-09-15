using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Contracts.Jwt
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
