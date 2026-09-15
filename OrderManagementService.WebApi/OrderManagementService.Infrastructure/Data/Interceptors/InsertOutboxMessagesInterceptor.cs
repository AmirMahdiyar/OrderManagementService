using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Infrastructure.Outbox;

namespace OrderManagementService.Infrastructure.Data.Interceptors
{
    public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                InsertOutboxMessages(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is not null)
                InsertOutboxMessages(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        private static void InsertOutboxMessages(DbContext context)
        {
            var aggregates = context.ChangeTracker
                .Entries<AggregateRoot<Guid>>()
                .Where(a => a.Entity.DomainEvents.Any())
                .Select(a => a.Entity)
                .ToList();

            var outboxMessages = aggregates
                .SelectMany(a =>
                {
                    var domainEvents = a.DomainEvents.ToList();
                    a.ClearDomainEvents();
                    return domainEvents;
                })
                .Select(domainEvent => OutboxMessage.Create(
                    type: domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                    content: JsonConvert.SerializeObject(domainEvent, OutboxSerializer.Settings)))
                .ToList();

            context.Set<OutboxMessage>()
                .AddRange(outboxMessages);
        }
    }
}
