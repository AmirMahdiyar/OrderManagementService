using OrderManagementService.Application.Base;
using OrderManagementService.Application.Dtos;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : CommandBase<CreateOrderCommandResponse>
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();

        public override void Validate()
        {
            new CreateOrderCommandValidator().Validate(this).ThrowIfNeeded();
        }
    }
}
