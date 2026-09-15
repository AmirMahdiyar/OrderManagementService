using FluentValidation;

namespace OrderManagementService.Application.Commands.ShipOrder
{
    public class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
    {
        public ShipOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        }
    }

}
