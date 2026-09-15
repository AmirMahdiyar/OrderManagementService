using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class DuplicateProductNotAllowedValidation : DomainValidation<IEnumerable<OrderItem>, DuplicateProductException>
    {
        private readonly Guid _productId;

        public DuplicateProductNotAllowedValidation(IEnumerable<OrderItem> value, Guid productId) : base(value)
        {
            _productId = productId;
        }

        protected override bool IsValid() => !Value.Any(i => i.ProductId == _productId);
    }
}
