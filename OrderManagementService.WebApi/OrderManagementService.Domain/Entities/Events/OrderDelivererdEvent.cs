using OrderManagementService.Domain.Entities.Base.Events;

namespace OrderManagementService.Domain.Entities.Events
{
    public record OrderDeliveredEvent(Guid OrderId) : IDomainEvent;
}
