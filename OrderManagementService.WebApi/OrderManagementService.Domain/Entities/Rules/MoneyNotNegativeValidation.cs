using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class MoneyNotNegativeValidation : DomainValidation<decimal, MoneyCannotBeNegativeException>
    {
        public MoneyNotNegativeValidation(decimal value) : base(value) { }
        protected override bool IsValid() => Value >= 0;
    }
}
