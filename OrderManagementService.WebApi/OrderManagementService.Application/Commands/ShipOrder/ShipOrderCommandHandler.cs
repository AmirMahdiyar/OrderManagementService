using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using ResponseModel = OrderManagementService.Application.Commands.ShipOrder.ShipOrderCommandResponse;
namespace OrderManagementService.Application.Commands.ShipOrder
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, ShipOrderCommandResponse>
    {
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShipOrderCommandHandler(
            IOrderCommandRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ShipOrderCommandResponse> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await CheckOrderExistance(request, cancellationToken);

            order.Ship();

            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .WithSuccess(true);
        }


        #region Private Methods
        private async Task<Order> CheckOrderExistance(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new InputValidationFailedApplicationException("Order not found.");
            return order;
        }
        #endregion
    }

}
