using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Queries.GetOrderById
{
    public static class GetOrderByIdQueryMapper
    {
        public static OrderItemResponse ToResponse(this OrderItem item)
        {
            return new OrderItemResponse(
                ProductId: item.ProductId,
                UnitPrice: item.UnitPrice.Value,
                Quantity: item.Quantity.Value
            );
        }

        public static GetOrderByIdQueryResponse ToResponse(this Order order)
        {
            return new GetOrderByIdQueryResponse(
                Id: order.Id,
                CustomerId: order.CustomerId,
                Status: order.State.Name,
                TotalAmount: order.TotalAmount.Value,
                CreatedDate: order.CreatedDate,
                Items: order.Items.Select(i => i.ToResponse()).ToList()
            );
        }
    }
}
