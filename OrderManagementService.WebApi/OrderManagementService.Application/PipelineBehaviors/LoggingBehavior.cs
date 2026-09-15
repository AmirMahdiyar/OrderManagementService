using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OrderManagementService.Application.PipelineBehaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var correlationId = Guid.NewGuid();

            var requestPayload = SerializePayload(request);
            _logger.LogInformation(
                "[{CorrelationId}] Handling {RequestName} | Request: {RequestPayload}",
                correlationId,
                requestName,
                requestPayload);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                var responsePayload = SerializePayload(response);
                _logger.LogInformation(
                    "[{CorrelationId}] Handled {RequestName} in {ElapsedMilliseconds} ms | Response: {ResponsePayload}",
                    correlationId,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    responsePayload);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(
                    ex,
                    "[{CorrelationId}] Failed handling {RequestName} after {ElapsedMilliseconds} ms | Error: {ErrorMessage}",
                    correlationId,
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }

        private static string SerializePayload(object? payload)
        {
            if (payload == null)
                return "null";

            try
            {
                if (payload is Commands.Login.LoginCommand loginCmd)
                {
                    return JsonSerializer.Serialize(new
                    {
                        loginCmd.Username,
                        Password = "***MASKED***"
                    }, JsonOptions);
                }

                return JsonSerializer.Serialize(payload, JsonOptions);
            }
            catch
            {
                return payload.ToString() ?? "unserializable";
            }
        }
    }
}
