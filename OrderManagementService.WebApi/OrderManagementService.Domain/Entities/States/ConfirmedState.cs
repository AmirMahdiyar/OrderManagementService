using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Domain.Entities.States
{
    public class ConfirmedState : OrderState
    {
        public override string Name => "Confirmed";

        public override void Ship(Order order)
        {
            order.SetState(new ShippedState());
        }
    }
}
