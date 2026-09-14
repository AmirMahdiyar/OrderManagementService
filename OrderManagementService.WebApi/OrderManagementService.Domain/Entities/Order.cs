using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Domain.Entities.Events;
using OrderManagementService.Domain.Entities.Rules;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Entities.States.Base;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.Domain.Services;

namespace OrderManagementService.Domain.Entities
{
    public class Order : AggregateRoot<Guid>
    {
        private OrderState _state;
        private List<OrderItem> _items = new();

        public DateTime CreatedDate { get; private set; }
        public OrderState State => _state;


        public Money TotalAmount => Money.Create(_items.Sum(i => (i.Quantity * i.UnitPrice).Value));

        protected Order() { } //For EF

        protected Order(Guid customerId, DateTime createdDate)
        {
            Id = InitialId();
            CustomerId = customerId;
            CreatedDate = createdDate;
            _state = new PendingState();
        }
        public Guid CustomerId { get; private set; }
        public IEnumerable<OrderItem> Items => _items.AsReadOnly();


        public static Order Create(Guid customerId)
            => new Order(customerId, DateTime.UtcNow);


        #region Behaviors
        internal void SetState(OrderState state)
        {
            _state = state;
        }

        public void AddOrderItem(Guid productId, Money unitPrice, Quantity quantity)
        {
            new OrderStatusMustBePendingForModificationValidation(State).Validate();
            new DuplicateProductNotAllowedValidation(_items, productId).Validate();

            var item = OrderItem.Create(Id, productId, unitPrice, quantity);
            _items.Add(item);

            AddDomainEvent(new OrderUpdatedEvent(Id));
        }

        public void RemoveOrderItem(Guid orderItemId)
        {
            new OrderStatusMustBePendingForModificationValidation(State).Validate();
            new OrderItemMustExistValidation(_items, orderItemId).Validate();

            var itemToRemove = _items.First(i => i.Id == orderItemId);
            _items.Remove(itemToRemove);

            AddDomainEvent(new OrderUpdatedEvent(Id));
        }

        public async Task ConfirmAsync(IOrderInventoryChecker inventoryChecker)
        {
            new OrderMustHaveItemsValidation(_items).Validate();

            bool hasStock = await inventoryChecker.HasSufficientStockAsync(_items);
            new InventoryMustBeSufficientValidation(hasStock).Validate();

            _state.Confirm(this);

            AddDomainEvent(new OrderConfirmedEvent(Id, _items.ToList().AsReadOnly()));
        }

        public void Ship()
        {
            _state.Ship(this);
            AddDomainEvent(new OrderShippedEvent(Id));
        }

        public void Deliver()
        {
            _state.Deliver(this);
            AddDomainEvent(new OrderDeliveredEvent(Id));
        }
        #endregion

        protected override Guid InitialId()
            => Guid.NewGuid();
    }
}
