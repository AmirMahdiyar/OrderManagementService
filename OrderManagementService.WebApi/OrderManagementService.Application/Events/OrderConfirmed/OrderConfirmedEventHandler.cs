using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Domain.Entities.Events;

namespace OrderManagementService.Application.Events.OrderConfirmed
{
    public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly IProductCommandRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderConfirmedEventHandler(IProductCommandRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            var productIds = notification.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _productRepository.GetProductsByIdsAsync(productIds, cancellationToken);

            foreach (var item in notification.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                    product.DecreaseStock(item.Quantity);
            }
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
