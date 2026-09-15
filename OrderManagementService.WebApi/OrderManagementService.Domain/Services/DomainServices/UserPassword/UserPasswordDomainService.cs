using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Domain.Services.DomainServices.UserPassword
{
    public class UserPasswordDomainService : IUserPasswordDomainService
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserPasswordDomainService(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public bool VerifyPassword(User user, string plainPassword)
        {
            return _passwordHasher.Verify(plainPassword, user.PasswordHash);
        }
    }
}
