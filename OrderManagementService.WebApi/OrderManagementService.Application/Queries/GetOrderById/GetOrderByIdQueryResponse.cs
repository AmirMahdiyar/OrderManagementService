namespace OrderManagementService.Application.Queries.GetOrderById
{
    public record OrderItemResponse(
        Guid ProductId,
        decimal UnitPrice,
        int Quantity
    );

    public record GetOrderByIdQueryResponse(
        Guid Id,
        Guid CustomerId,
        string Status,
        decimal TotalAmount,
        DateTime CreatedDate,
        List<OrderItemResponse> Items
    );
}

