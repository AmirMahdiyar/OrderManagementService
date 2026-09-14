using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class OrderMustHaveItemsException : DomainException
    {
        public OrderMustHaveItemsException() : base("Order must have at least one item.") { }
    }
}
