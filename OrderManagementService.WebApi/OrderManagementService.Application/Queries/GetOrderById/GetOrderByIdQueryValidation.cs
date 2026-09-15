using FluentValidation;

namespace OrderManagementService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryValidation : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidation()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        }
    }
}
