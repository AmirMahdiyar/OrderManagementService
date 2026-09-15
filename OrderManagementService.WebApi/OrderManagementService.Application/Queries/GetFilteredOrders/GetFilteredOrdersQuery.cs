using OrderManagementService.Application.Base;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Extensions;

namespace OrderManagementService.Application.Queries.GetFilteredOrders
{
    public class GetFilteredOrdersQuery : QueryBase<PaginationDto<GetFilteredOrdersQueryResponse>>
    {
        public PaginationQuery Pagination { get; set; } = new PaginationQuery();
        public Guid? CustomerId { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override void Validate()
        {
            new GetFilteredOrdersQueryValidation().Validate(this).ThrowIfNeeded();
        }
    }
}
