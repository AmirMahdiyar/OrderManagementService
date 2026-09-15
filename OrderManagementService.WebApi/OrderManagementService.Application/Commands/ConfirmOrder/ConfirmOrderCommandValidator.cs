using FluentValidation;

namespace OrderManagementService.Application.Commands.ConfirmOrder
{
    public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
    {
        public ConfirmOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        }
    }

}
