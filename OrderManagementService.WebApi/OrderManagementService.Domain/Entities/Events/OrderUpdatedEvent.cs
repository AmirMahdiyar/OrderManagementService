using OrderManagementService.Domain.Entities.Base.Events;

namespace OrderManagementService.Domain.Entities.Events
{
    public record OrderUpdatedEvent(Guid OrderId) : IDomainEvent;
}
