using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Domain.Entities.States
{
    public class DeliveredState : OrderState
    {
        public override string Name => "Delivered";
    }
}
