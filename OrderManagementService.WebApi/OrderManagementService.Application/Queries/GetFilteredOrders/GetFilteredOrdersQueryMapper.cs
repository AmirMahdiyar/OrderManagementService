using OrderManagementService.Application.Common;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Queries.GetFilteredOrders
{
    public static class GetFilteredOrdersQueryMapper
    {
        public static GetFilteredOrdersQueryResponse ToResponse(this Order order)
        {
            return new GetFilteredOrdersQueryResponse(
                Id: order.Id,
                CustomerId: order.CustomerId,
                Status: order.State.Name,
                TotalAmount: order.TotalAmount.Value,
                CreatedDate: order.CreatedDate,
                ItemsCount: order.Items.Count()
            );
        }

        public static PaginationDto<GetFilteredOrdersQueryResponse> ToResponse(this PaginationDto<Order> pagination)
        {
            return new PaginationDto<GetFilteredOrdersQueryResponse>(
                Items: pagination.Items.Select(x => x.ToResponse()),
                TotalCount: pagination.TotalCount,
                CurrentPage: pagination.CurrentPage,
                TotalPages: pagination.TotalPages,
                HasNextPage: pagination.HasNextPage,
                HasPreviousPage: pagination.HasPreviousPage
            );
        }
    }
}
