using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class OrderItemMustExistValidation : DomainValidation<IEnumerable<OrderItem>, OrderItemNotFoundException>
    {
        private readonly Guid _orderItemId;
        public OrderItemMustExistValidation(IEnumerable<OrderItem> value, Guid orderItemId) : base(value)
        {
            _orderItemId = orderItemId;
        }
        protected override bool IsValid() => Value.Any(i => i.Id == _orderItemId);
    }
}
