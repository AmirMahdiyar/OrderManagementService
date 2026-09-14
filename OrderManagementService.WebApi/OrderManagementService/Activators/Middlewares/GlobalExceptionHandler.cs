using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;
using OrderManagementService.Domain.Exceptions.PresentationExceptions.Base;
using static OrderManagementService.Activators.Middlewares.Constants.GlobalExceptionHandlerConstants;
namespace OrderManagementService.Activators.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, LogErrorMessage, exception.Message);

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            if (exception is DomainException domainException)
                FillHttpContext(httpContext, problemDetails, domainException);

            else if (exception is ApplicationException applicationException)
                FillHttpContext(httpContext, problemDetails, applicationException);

            else if (exception is PresentationException presentationException)
                FillHttpContext(httpContext, problemDetails, presentationException);

            else
                FillHttpContext(httpContext, problemDetails);

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }


        #region Private Methods
        private static void FillHttpContext(HttpContext httpContext, ProblemDetails problemDetails)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Title = LogErrorMessage;
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = LogErrorMessage;
        }

        private static void FillHttpContext(HttpContext httpContext, ProblemDetails problemDetails, PresentationException presentationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = HandledPresentationExceptionMessageTitle;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = presentationException.Message;
        }

        private static void FillHttpContext(HttpContext httpContext, ProblemDetails problemDetails, ApplicationException applicationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = HandledApplicationExceptionMessageTitle;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = applicationException.Message;
        }

        private static void FillHttpContext(HttpContext httpContext, ProblemDetails problemDetails, DomainException domainException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = HandledDomainExceptionMessageTitle;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = domainException.Message;
        }
        #endregion
    }
}
