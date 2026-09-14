using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Domain.Entities.States
{
    public class PendingState : OrderState
    {
        public override string Name => "Pending";

        public override void Confirm(Order order)
        {
            order.SetState(new ConfirmedState());
        }
    }
}
