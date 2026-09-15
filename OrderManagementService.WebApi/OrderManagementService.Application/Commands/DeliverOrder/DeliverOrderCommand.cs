using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.DeliverOrder
{
    public class DeliverOrderCommand : CommandBase<DeliverOrderCommandResponse>
    {
        public Guid OrderId { get; set; }
        public override void Validate()
        {
            new DeliverOrderCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
