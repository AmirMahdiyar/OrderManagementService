using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class QuantityMustBeGreaterThanZeroException : DomainException
    {
        public QuantityMustBeGreaterThanZeroException() : base("Quantity must be greater than zero.") { }
    }
}
