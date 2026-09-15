using MediatR;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;

namespace OrderManagementService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryResponse>
    {
        private readonly IOrderQueryRepository _queryRepository;

        public GetOrderByIdQueryHandler(IOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<GetOrderByIdQueryResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await GetOrder(request, cancellationToken);

            return order.ToResponse();
        }


        #region Private Methods
        private async Task<Order> GetOrder(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _queryRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new InputValidationFailedApplicationException("Order not found.");
            return order;
        }
        #endregion
    }
}
