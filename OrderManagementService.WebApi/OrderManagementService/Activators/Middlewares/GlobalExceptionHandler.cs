using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Application.Exceptions;
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
            switch (exception)
            {
                case InputValidationFailedApplicationException validationException:
                    FillHttpContext(httpContext, problemDetails, validationException);
                    break;
                case DomainException domainException:
                    FillHttpContext(httpContext, problemDetails, domainException);
                    break;
                case ApplicationException applicationException:
                    FillHttpContext(httpContext, problemDetails, applicationException);
                    break;
                case PresentationException presentationException:
                    FillHttpContext(httpContext, problemDetails, presentationException);
                    break;
                case DbUpdateConcurrencyException concurrencyException:
                    FillConcurrencyHttpContext(httpContext, problemDetails, concurrencyException);
                    break;
                default:
                    FillHttpContext(httpContext, problemDetails);
                    break;
            }
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }



        #region Private Methods
        private static void FillHttpContext(HttpContext httpContext, ProblemDetails problemDetails, InputValidationFailedApplicationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = HandledValidationExceptionMessageTitle;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = validationException.Message;
        }
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

        private static void FillConcurrencyHttpContext(HttpContext httpContext, ProblemDetails problemDetails, DbUpdateConcurrencyException concurrencyException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            problemDetails.Title = HandledConcurrencyExceptionMessageTitle;
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Detail = ConcurrencyConflictErrorMessage;
        }
        #endregion
    }
}
