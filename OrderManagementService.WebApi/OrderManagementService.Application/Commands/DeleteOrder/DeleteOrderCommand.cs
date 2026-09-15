using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommand : CommandBase<DeleteOrderCommandResponse>
    {
        public Guid OrderId { get; set; }
        public override void Validate()
        {
            new DeleteOrderCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
