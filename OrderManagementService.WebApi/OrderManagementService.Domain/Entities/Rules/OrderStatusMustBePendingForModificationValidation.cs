using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Entities.States.Base;
using OrderManagementService.Domain.Entities.States.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class OrderStatusMustBePendingForModificationValidation : DomainValidation<OrderState, InvalidOrderStateException>
    {
        public OrderStatusMustBePendingForModificationValidation(OrderState value) : base(value) { }

        protected override bool IsValid() => Value is PendingState;
    }
}
