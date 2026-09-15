using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.UnitTests.Common.Helpers;

namespace OrderManagementService.UnitTests.Common.Builders
{
    public class OrderBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _customerId = Guid.NewGuid();
        private readonly List<(Guid ProductId, Money UnitPrice, Quantity Quantity)> _items = new();
        private string _targetState = "Pending";

        public OrderBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public OrderBuilder WithCustomerId(Guid customerId)
        {
            _customerId = customerId;
            return this;
        }

        public OrderBuilder WithItem(Guid productId, decimal unitPrice, int quantity)
        {
            _items.Add((productId, Money.Create(unitPrice), Quantity.Create(quantity)));
            return this;
        }

        public OrderBuilder WithItem(Guid productId, Money unitPrice, Quantity quantity)
        {
            _items.Add((productId, unitPrice, quantity));
            return this;
        }

        public OrderBuilder InConfirmedState()
        {
            _targetState = "Confirmed";
            return this;
        }

        public OrderBuilder InShippedState()
        {
            _targetState = "Shipped";
            return this;
        }

        public OrderBuilder InDeliveredState()
        {
            _targetState = "Delivered";
            return this;
        }

        public Order Build()
        {
            var order = Order.Create(_customerId);
            order.WithId(_id);

            foreach (var item in _items)
            {
                order.AddOrderItem(item.ProductId, item.UnitPrice, item.Quantity);
            }

            if (_targetState == "Confirmed")
            {
                order.Confirm();
            }
            else if (_targetState == "Shipped")
            {
                order.Confirm();
                order.Ship();
            }
            else if (_targetState == "Delivered")
            {
                order.Confirm();
                order.Ship();
                order.Deliver();
            }

            return order;
        }
    }
}
