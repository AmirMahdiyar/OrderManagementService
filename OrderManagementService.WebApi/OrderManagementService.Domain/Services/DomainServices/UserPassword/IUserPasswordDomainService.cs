using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.Services.DomainServices.UserPassword
{
    public interface IUserPasswordDomainService
    {
        bool VerifyPassword(User user, string plainPassword);
    }
}
