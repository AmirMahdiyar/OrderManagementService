using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderManagementService.Domain.Entities.Events;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Outbox;

namespace OrderManagementService.Activators.Jobs
{
    public class OutboxBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OutboxBackgroundService> _logger;

        public OutboxBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessages(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox messages.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        #region Private Methods

        private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedOn == null && m.Error == null)
                .OrderBy(m => m.OccurredOn)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (!messages.Any())
                return;

            foreach (var message in messages)
            {
                try
                {
                    var eventType = Type.GetType(message.Type)
                        ?? AppDomain.CurrentDomain.GetAssemblies()
                            .Select(a => a.GetType(message.Type))
                            .FirstOrDefault(t => t != null)
                        ?? typeof(OrderConfirmedEvent).Assembly.GetTypes()
                            .FirstOrDefault(t => t.Name == message.Type || t.FullName == message.Type);

                    if (eventType == null)
                    {
                        _logger.LogWarning("Could not resolve type: {Type}", message.Type);
                        message.MarkAsFailed($"Type {message.Type} not found.");
                        continue;
                    }

                    var domainEvent = JsonConvert.DeserializeObject(message.Content, eventType, OutboxSerializer.Settings);

                    if (domainEvent == null)
                    {
                        _logger.LogWarning("Could not deserialize payload for message {Id}", message.Id);
                        message.MarkAsFailed("Deserialization failed.");
                        continue;
                    }

                    await publisher.Publish(domainEvent, stoppingToken);

                    message.MarkAsProcessed();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {Id}", message.Id);
                    message.MarkAsFailed(ex.Message);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
        #endregion
    }
}
