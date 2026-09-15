using OrderManagementService.Domain.Entities.Base.Events;

namespace OrderManagementService.Domain.Entities.Events
{
    public record OrderDeletedEvent(Guid OrderId, IEnumerable<OrderItem> Items, string State) : IDomainEvent;
}
