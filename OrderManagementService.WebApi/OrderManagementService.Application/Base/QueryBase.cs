using MediatR;

namespace OrderManagementService.Application.Base
{
    public abstract class QueryBase<TResponse> : IRequest<TResponse>, IValidatableRequest
    {
        public abstract void Validate();
    }
}

