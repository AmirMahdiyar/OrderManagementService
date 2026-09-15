using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.ConfirmOrder
{

    public class ConfirmOrderCommand : CommandBase<ConfirmOrderCommandResponse>
    {
        public Guid OrderId { get; set; }

        public override void Validate()
        {
            new ConfirmOrderCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
