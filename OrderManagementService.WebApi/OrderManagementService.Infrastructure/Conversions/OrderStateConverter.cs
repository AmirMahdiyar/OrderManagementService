using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Infrastructure.Conversions
{
    public static class OrderStateConverter
    {
        public static OrderState ConvertToState(string stateName) => stateName switch
        {
            "Pending" => new PendingState(),
            "Confirmed" => new ConfirmedState(),
            "Shipped" => new ShippedState(),
            "Delivered" => new DeliveredState(),
            _ => throw new InvalidOperationException($"Unknown state: {stateName}")
        };
    }
}
