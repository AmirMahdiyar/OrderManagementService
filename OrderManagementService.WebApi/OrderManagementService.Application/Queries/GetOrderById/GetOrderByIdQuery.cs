using OrderManagementService.Application.Base;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQuery : QueryBase<GetOrderByIdQueryResponse>
    {
        public Guid OrderId { get; set; }

        public override void Validate()
        {
            new GetOrderByIdQueryValidation().Validate(this).ThrowIfNeeded();
        }
    }
}
