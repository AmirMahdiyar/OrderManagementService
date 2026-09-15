using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;
using ResponseModel = OrderManagementService.Application.Commands.BulkInsertOrders.BulkInsertOrdersCommandResponse;
namespace OrderManagementService.Application.Commands.BulkInsertOrders
{
    public class BulkInsertOrdersCommandHandler : IRequestHandler<BulkInsertOrdersCommand, BulkInsertOrdersCommandResponse>
    {
        private readonly ICustomerCommandRepository _customerRepository;
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IProductCommandRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BulkInsertOrdersCommandHandler(
            ICustomerCommandRepository customerRepository,
            IOrderCommandRepository orderRepository,
            IProductCommandRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<BulkInsertOrdersCommandResponse> Handle(BulkInsertOrdersCommand request, CancellationToken cancellationToken)
        {
            await CheckCustomerExistance(request, cancellationToken);

            var products = await GetProducts(request, cancellationToken);

            var productDict = products.ToDictionary(p => p.Id);
            var ordersToInsert = new List<Order>();

            foreach (var orderDto in request.Orders)
            {
                var order = CreateOrder(productDict, orderDto);

                ordersToInsert.Add(order);
            }

            _orderRepository.AddRange(ordersToInsert);

            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .WithInsertedCount(ordersToInsert.Count);
        }


        #region Private Methods
        private static Order CreateOrder(Dictionary<Guid, Product> productDict, Dtos.BulkOrderDto orderDto)
        {
            var order = Order.Create(orderDto.CustomerId);

            foreach (var item in orderDto.Items)
            {
                var product = productDict[item.ProductId];
                order.AddOrderItem(
                    product.Id,
                    product.Price,
                    Quantity.Create(item.Quantity));
            }

            return order;
        }

        private async Task<List<Product>> GetProducts(BulkInsertOrdersCommand request, CancellationToken cancellationToken)
        {
            var productIds = request.Orders.SelectMany(o => o.Items).Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.GetProductsByIdsAsync(productIds, cancellationToken);
            if (products.Count != productIds.Count)
                throw new InputValidationFailedApplicationException("One or more Products do not exist.");
            return products;
        }

        private async Task CheckCustomerExistance(BulkInsertOrdersCommand request, CancellationToken cancellationToken)
        {
            var customerIds = request.Orders.Select(o => o.CustomerId).Distinct().ToList();
            var customersExist = await _customerRepository.ExistsAsync(customerIds, cancellationToken);
            if (!customersExist)
                throw new InputValidationFailedApplicationException("One or more Customers do not exist.");
        }
        #endregion
    }

}
