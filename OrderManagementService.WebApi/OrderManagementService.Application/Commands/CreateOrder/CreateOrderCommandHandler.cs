using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;
using ResponseModel = OrderManagementService.Application.Commands.CreateOrder.CreateOrderCommandResponse;

namespace OrderManagementService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResponse>
    {
        private readonly ICustomerCommandRepository _customerRepository;
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IProductCommandRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(ICustomerCommandRepository customerRepository, IUnitOfWork unitOfWork, IOrderCommandRepository orderRepository, IProductCommandRepository productRepository)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await CheckCustomerExistance(request, cancellationToken);
            var products = await GetProductsAsync(request, cancellationToken);
            var productDict = products.ToDictionary(p => p.Id);

            Order order = CreateOrder(request, productDict);

            _orderRepository.Add(order);
            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .Succeeded()
                .WithOrderId(order.Id);
        }




        #region Private Methods
        private static Order CreateOrder(CreateOrderCommand request, Dictionary<Guid, Product> productDict)
        {
            var order = Order.Create(request.CustomerId);

            foreach (var item in request.Items)
            {
                var product = productDict[item.ProductId];
                order.AddOrderItem(
                    product.Id,
                    product.Price,
                    Quantity.Create(item.Quantity));
            }

            return order;
        }
        private async Task CheckCustomerExistance(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var customerExists = await _customerRepository.ExistsAsync(request.CustomerId, cancellationToken);
            if (!customerExists)
                throw new InputValidationFailedApplicationException("Customer not found.");
        }
        private async Task<List<Product>> GetProductsAsync(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var productIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.GetProductsByIdsAsync(productIds, cancellationToken);

            if (products.Count != productIds.Count)
                throw new InputValidationFailedApplicationException("One or more products do not exist.");

            return products;
        }
        #endregion
    }

}
