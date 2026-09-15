using OrderManagementService.Infrastructure.Outbox.Exceptions;

namespace OrderManagementService.Infrastructure.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Type { get; private set; }
        public string Content { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public DateTime? ProcessedOn { get; private set; }
        public string? Error { get; private set; }

        protected OutboxMessage() { } //For EF

        private OutboxMessage(string type, string content, DateTime occurredOn)
        {
            CheckType(type);
            CheckContent(content);

            Id = Guid.NewGuid();
            Type = type;
            Content = content;
            OccurredOn = occurredOn;
        }

        public static OutboxMessage Create(string type, string content)
            => new OutboxMessage(type, content, DateTime.Now);

        public void MarkAsProcessed()
        {
            ProcessedOn = DateTime.Now;
            Error = null;
        }

        public void MarkAsFailed(string error)
        {
            ProcessedOn = DateTime.Now;
            Error = error;
        }
        #region Private Methods
        private void CheckType(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new TypeCanNotBeNullOrEmpty();
        }

        private void CheckContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                throw new ContentCanNotBeNullOrEmpty();
        }
        #endregion
    }
}
