using OrderManagementService.Domain.Entities.Base.Events;

namespace OrderManagementService.Domain.Entities.Base.Entity
{
    public abstract class AggregateRoot<TId> : BaseEntity<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IEnumerable<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
