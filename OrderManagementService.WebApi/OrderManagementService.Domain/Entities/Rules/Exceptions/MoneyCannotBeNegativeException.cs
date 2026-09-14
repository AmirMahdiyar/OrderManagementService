using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class MoneyCannotBeNegativeException : DomainException
    {
        public MoneyCannotBeNegativeException() : base("Amount cannot be negative.") { }
    }
}
