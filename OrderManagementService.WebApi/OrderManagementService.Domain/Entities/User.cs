using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Domain.Entities.Rules;
using OrderManagementService.Domain.Services;

namespace OrderManagementService.Domain.Entities
{
    public class User : AggregateRoot<Guid>
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        protected User() { }

        protected User(string username, string plainPassword, string role, IPasswordHasher passwordHasher)
        {
            new StringNotNullOrEmptyValidation(username).Validate();
            new StringNotNullOrEmptyValidation(plainPassword).Validate();
            new StringNotNullOrEmptyValidation(role).Validate();

            Id = InitialId();
            Username = username;
            PasswordHash = passwordHasher.Hash(plainPassword);
            Role = role;
        }

        public static User Create(string username, string plainPassword, string role, IPasswordHasher passwordHasher)
            => new User(username, plainPassword, role, passwordHasher);

        protected override Guid InitialId()
            => Guid.NewGuid();
    }
}
