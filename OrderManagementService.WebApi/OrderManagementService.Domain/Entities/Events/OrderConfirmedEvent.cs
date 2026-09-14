using OrderManagementService.Domain.Entities.Base.Events;

namespace OrderManagementService.Domain.Entities.Events
{
    public record OrderConfirmedEvent(Guid OrderId, IEnumerable<OrderItem> Items) : IDomainEvent;
}
