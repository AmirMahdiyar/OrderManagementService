using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using ResponseModel = OrderManagementService.Application.Commands.DeleteOrder.DeleteOrderCommandResponse;
namespace OrderManagementService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, DeleteOrderCommandResponse>
    {
        private readonly IOrderCommandRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOrderCommandHandler(IOrderCommandRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<DeleteOrderCommandResponse> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await GetOrder(request, cancellationToken);

            order.Delete();

            _orderRepository.Delete(order);

            var result = await _unitOfWork.CommitAsync(cancellationToken);
            result.ThrowIfNoChanges<NoChangesApplicationException>();

            return ResponseModel
                .Response()
                .WithSuccess(true);
        }


        #region Private Methods
        private async Task<Order> GetOrder(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new InputValidationFailedApplicationException("Order not found.");
            return order;
        }
        #endregion
    }

}
