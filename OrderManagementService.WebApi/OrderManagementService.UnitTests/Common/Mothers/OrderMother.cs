using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Builders;

namespace OrderManagementService.UnitTests.Common.Mothers
{
    public static class OrderMother
    {
        public static Order PendingOrderWithOneItem(Guid customerId, Guid productId, decimal price = 100m, int qty = 1)
            => new OrderBuilder()
                .WithCustomerId(customerId)
                .WithItem(productId, price, qty)
                .Build();

        public static Order EmptyPendingOrder(Guid? customerId = null)
            => new OrderBuilder()
                .WithCustomerId(customerId ?? Guid.NewGuid())
                .Build();

        public static Order ConfirmedOrder(Guid customerId, Guid productId, decimal price = 100m, int qty = 1)
            => new OrderBuilder()
                .WithCustomerId(customerId)
                .WithItem(productId, price, qty)
                .InConfirmedState()
                .Build();

        public static Order ShippedOrder(Guid customerId, Guid productId, decimal price = 100m, int qty = 1)
            => new OrderBuilder()
                .WithCustomerId(customerId)
                .WithItem(productId, price, qty)
                .InShippedState()
                .Build();

        public static Order DeliveredOrder(Guid customerId, Guid productId, decimal price = 100m, int qty = 1)
            => new OrderBuilder()
                .WithCustomerId(customerId)
                .WithItem(productId, price, qty)
                .InDeliveredState()
                .Build();
    }
}
