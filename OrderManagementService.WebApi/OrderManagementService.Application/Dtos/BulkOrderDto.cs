namespace OrderManagementService.Application.Dtos
{
    public record BulkOrderDto(Guid CustomerId, List<OrderItemDto> Items);
}
