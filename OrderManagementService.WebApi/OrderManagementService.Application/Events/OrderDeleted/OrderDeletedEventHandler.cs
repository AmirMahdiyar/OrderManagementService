using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Domain.Entities.Events;

namespace OrderManagementService.Application.Events.OrderDeleted
{
    public class OrderDeletedEventHandler : INotificationHandler<OrderDeletedEvent>
    {
        private readonly IProductCommandRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderDeletedEventHandler(IProductCommandRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
        {
            if (!string.Equals(notification.State, "Confirmed", StringComparison.OrdinalIgnoreCase))
                return;

            var productIds = notification.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _productRepository.GetProductsByIdsAsync(productIds, cancellationToken);

            foreach (var item in notification.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                    product.IncreaseStock(item.Quantity);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
