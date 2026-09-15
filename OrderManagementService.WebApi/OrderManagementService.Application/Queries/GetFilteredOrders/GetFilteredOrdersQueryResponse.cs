namespace OrderManagementService.Application.Queries.GetFilteredOrders
{
    public record GetFilteredOrdersQueryResponse(
            Guid Id,
            Guid CustomerId,
            string Status,
            decimal TotalAmount,
            DateTime CreatedDate,
            int ItemsCount);
}
