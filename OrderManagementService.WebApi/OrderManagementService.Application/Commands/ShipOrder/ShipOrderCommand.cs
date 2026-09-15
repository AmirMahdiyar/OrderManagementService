using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.ShipOrder
{
    public class ShipOrderCommand : CommandBase<ShipOrderCommandResponse>
    {
        public Guid OrderId { get; set; }
        public override void Validate()
        {
            new ShipOrderCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
