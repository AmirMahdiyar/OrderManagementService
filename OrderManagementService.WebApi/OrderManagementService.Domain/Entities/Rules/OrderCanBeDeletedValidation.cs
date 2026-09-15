using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Entities.States.Base;
using OrderManagementService.Domain.Entities.States.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class OrderCanBeDeletedValidation : DomainValidation<OrderState, InvalidOrderStateException>
    {
        public OrderCanBeDeletedValidation(OrderState value) : base(value) { }

        protected override bool IsValid() => Value is not ShippedState && Value is not DeliveredState;
    }
}
