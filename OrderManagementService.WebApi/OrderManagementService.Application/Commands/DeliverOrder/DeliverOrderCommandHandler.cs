using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using ResponseModel = OrderManagementService.Application.Commands.DeliverOrder.DeliverOrderCommandResponse;
namespace OrderManagementService.Application.Commands.DeliverOrder
{
    public class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, DeliverOrderCommandResponse>
    {
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeliverOrderCommandHandler(
            IOrderCommandRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeliverOrderCommandResponse> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await CheckOrderExistance(request, cancellationToken);

            order.Deliver();

            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .WithSuccess(true);
        }

        private async Task<Order> CheckOrderExistance(DeliverOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new InputValidationFailedApplicationException("Order not found.");
            return order;
        }
    }

}
