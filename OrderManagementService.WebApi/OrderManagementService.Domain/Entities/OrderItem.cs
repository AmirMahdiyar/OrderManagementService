using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.Domain.Entities
{
    public class OrderItem : BaseEntity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Money UnitPrice { get; private set; }
        public Quantity Quantity { get; private set; }

        protected OrderItem() { } //For EF

        protected OrderItem(Guid orderId, Guid productId, Money unitPrice, Quantity quantity)
        {
            Id = InitialId();
            OrderId = orderId;
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public Guid ProductId { get; private set; }


        internal static OrderItem Create(Guid orderId, Guid productId, Money unitPrice, Quantity quantity)
            => new OrderItem(orderId, productId, unitPrice, quantity);

        protected override Guid InitialId()
            => Guid.NewGuid();
    }
}
