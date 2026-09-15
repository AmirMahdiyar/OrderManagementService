using FluentValidation;
using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Application.Queries.GetFilteredOrders
{
    public class GetFilteredOrdersQueryValidation : AbstractValidator<GetFilteredOrdersQuery>
    {
        public GetFilteredOrdersQueryValidation()
        {
            RuleFor(x => x.Pagination.Page).GreaterThan(0).WithMessage("Page must be greater than 0.");
            RuleFor(x => x.Pagination.Size).GreaterThan(0).WithMessage("Size must be greater than 0.");

            RuleFor(x => x.OrderStatus)
                .Must(status => OrderState.IsValid(status))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderStatus))
                .WithMessage("Order status does not exist. Valid statuses are: Pending, Confirmed, Shipped, Delivered.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("EndDate must be greater than or equal to StartDate.");
        }
    }
}
