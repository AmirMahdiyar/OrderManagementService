using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Services.DomainServices.OrderConfirmation;
using ResponseModel = OrderManagementService.Application.Commands.ConfirmOrder.ConfirmOrderCommandResponse;
namespace OrderManagementService.Application.Commands.ConfirmOrder
{
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, ConfirmOrderCommandResponse>
    {
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderConfirmationDomainService _orderConfirmationDomainService;

        public ConfirmOrderCommandHandler(
            IOrderCommandRepository orderRepository,
            IUnitOfWork unitOfWork,
            IOrderConfirmationDomainService orderConfirmationDomainService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _orderConfirmationDomainService = orderConfirmationDomainService;
        }
        public async Task<ConfirmOrderCommandResponse> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await GetOrderWithItems(request, cancellationToken);

            await _orderConfirmationDomainService.ConfirmAsync(order, cancellationToken);

            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .WithSuccess(true);
        }


        #region Private Methods
        private async Task<Order> GetOrderWithItems(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new InputValidationFailedApplicationException("Order not found.");
            return order;
        }
        #endregion
    }

}
