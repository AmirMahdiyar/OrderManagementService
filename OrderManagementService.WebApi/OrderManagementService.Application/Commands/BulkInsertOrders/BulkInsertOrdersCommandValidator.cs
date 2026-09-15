using FluentValidation;

namespace OrderManagementService.Application.Commands.BulkInsertOrders
{
    public class BulkInsertOrdersCommandValidator : AbstractValidator<BulkInsertOrdersCommand>
    {
        public BulkInsertOrdersCommandValidator()
        {
            RuleFor(x => x.Orders)
                .NotEmpty().WithMessage("Orders list cannot be empty.");

            RuleForEach(x => x.Orders).ChildRules(order =>
            {
                order.RuleFor(o => o.CustomerId).NotEmpty().WithMessage("Customer Id is required.");
                order.RuleFor(o => o.Items).NotEmpty().WithMessage("Order must contain at least one item.");

                order.RuleForEach(o => o.Items).ChildRules(items =>
                {
                    items.RuleFor(i => i.ProductId).NotEmpty();
                    items.RuleFor(i => i.Quantity).GreaterThan(0);
                });
            });
        }
    }

}
