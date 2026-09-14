using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Domain.Entities.States
{
    public class ShippedState : OrderState
    {
        public override string Name => "Shipped";

        public override void Deliver(Order order)
        {
            order.SetState(new DeliveredState());
        }
    }
}
