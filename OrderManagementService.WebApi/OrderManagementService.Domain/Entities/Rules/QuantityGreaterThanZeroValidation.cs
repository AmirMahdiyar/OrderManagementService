using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class QuantityGreaterThanZeroValidation : DomainValidation<int, QuantityMustBeGreaterThanZeroException>
    {
        public QuantityGreaterThanZeroValidation(int value) : base(value) { }
        protected override bool IsValid() => Value > 0;
    }
}
