using FluentValidation;

namespace OrderManagementService.Application.Commands.DeliverOrder
{
    public class DeliverOrderCommandValidator : AbstractValidator<DeliverOrderCommand>
    {
        public DeliverOrderCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        }
    }

}
