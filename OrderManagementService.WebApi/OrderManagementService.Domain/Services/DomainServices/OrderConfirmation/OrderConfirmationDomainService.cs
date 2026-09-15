using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Rules;

namespace OrderManagementService.Domain.Services.DomainServices.OrderConfirmation
{
    public class OrderConfirmationDomainService : IOrderConfirmationDomainService
    {
        private readonly IOrderInventoryChecker _inventoryChecker;

        public OrderConfirmationDomainService(IOrderInventoryChecker inventoryChecker)
        {
            _inventoryChecker = inventoryChecker;
        }

        public async Task ConfirmAsync(Order order, CancellationToken cancellationToken = default)
        {
            new OrderMustHaveItemsValidation(order.Items).Validate();

            bool hasStock = await _inventoryChecker.HasSufficientStockAsync(order.Items, cancellationToken);
            new InventoryMustBeSufficientValidation(hasStock).Validate();

            order.Confirm();
        }
    }
}
