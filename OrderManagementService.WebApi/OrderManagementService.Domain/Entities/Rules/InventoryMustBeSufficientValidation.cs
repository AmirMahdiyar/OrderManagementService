using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class InventoryMustBeSufficientValidation : DomainValidation<bool, InsufficientStockException>
    {
        public InventoryMustBeSufficientValidation(bool value) : base(value) { }
        protected override bool IsValid() => Value;
    }
}
