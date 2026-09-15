using MediatR;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.Application.Queries.GetFilteredOrders
{
    public class GetFilteredOrdersQueryHandler : IRequestHandler<GetFilteredOrdersQuery, PaginationDto<GetFilteredOrdersQueryResponse>>
    {
        private readonly IOrderQueryRepository _queryRepository;

        public GetFilteredOrdersQueryHandler(IOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<PaginationDto<GetFilteredOrdersQueryResponse>> Handle(GetFilteredOrdersQuery request, CancellationToken cancellationToken)
        {
            ValidateOrderStatus(request);

            var normalizedStatus = OrderState.Normalize(request.OrderStatus);

            var orders = await _queryRepository.GetFilteredOrdersAsync(
                request.Pagination,
                request.CustomerId,
                normalizedStatus,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            return orders.ToResponse();
        }

        #region Private Methods
        private static void ValidateOrderStatus(GetFilteredOrdersQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.OrderStatus) && !OrderState.IsValid(request.OrderStatus))
            {
                throw new InputValidationFailedApplicationException("Order status does not exist. Valid statuses are: Pending, Confirmed, Shipped, Delivered.");
            }
        }
        #endregion
    }
}
