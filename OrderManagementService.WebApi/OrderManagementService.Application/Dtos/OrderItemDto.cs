namespace OrderManagementService.Application.Dtos
{
    public record OrderItemDto(Guid ProductId, int Quantity);
}
