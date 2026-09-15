using MediatR;
using OrderManagementService.Application.Base;

namespace OrderManagementService.Application.PipelineBehaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IValidatableRequest validatableRequest)
                validatableRequest.Validate();

            return await next();
        }
    }
}
