using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Domain.Entities.Rules;

namespace OrderManagementService.Domain.Entities
{
    public class Customer : AggregateRoot<Guid>
    {
        public string FullName { get; private set; }
        public Guid UserId { get; private set; }

        protected Customer() { } //For EF

        protected Customer(string fullName, Guid userId)
        {
            new StringNotNullOrEmptyValidation(fullName).Validate();

            Id = InitialId();
            FullName = fullName;
            UserId = userId;
        }

        public static Customer Create(string fullName, Guid userId)
            => new Customer(fullName, userId);

        protected override Guid InitialId()
            => Guid.NewGuid();
    }
}
