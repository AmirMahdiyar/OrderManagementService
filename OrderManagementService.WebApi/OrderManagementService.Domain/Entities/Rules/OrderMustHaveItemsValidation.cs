using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class OrderMustHaveItemsValidation : DomainValidation<IEnumerable<OrderItem>, OrderMustHaveItemsException>
    {
        public OrderMustHaveItemsValidation(IEnumerable<OrderItem> value) : base(value) { }

        protected override bool IsValid() => Value.Any();
    }
}
