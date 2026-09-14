using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class InsufficientStockException : DomainException
    {
        public InsufficientStockException() : base("Insufficient stock for the product(s).") { }
    }
}
