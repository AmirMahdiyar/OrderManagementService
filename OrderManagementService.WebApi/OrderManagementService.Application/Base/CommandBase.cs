using MediatR;

namespace OrderManagementService.Application.Base
{
    public abstract class CommandBase : IRequest, IValidatableRequest
    {
        public abstract void Validate();
    }
    public abstract class CommandBase<TResponse> : IRequest<TResponse>, IValidatableRequest
    {
        public abstract void Validate();
    }
}

