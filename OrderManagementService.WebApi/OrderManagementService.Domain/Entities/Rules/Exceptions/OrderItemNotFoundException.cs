using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class OrderItemNotFoundException : DomainException
    {
        public OrderItemNotFoundException() : base("Order item not found.") { }
    }
}
