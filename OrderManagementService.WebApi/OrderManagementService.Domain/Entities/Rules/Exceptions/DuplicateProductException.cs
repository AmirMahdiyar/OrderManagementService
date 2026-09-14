using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class DuplicateProductException : DomainException
    {
        public DuplicateProductException() : base("This product is already added to the order.") { }
    }
}
