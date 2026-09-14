using OrderManagementService.Domain.Exceptions.Infrastructure;

namespace OrderManagementService.Infrastructure.Outbox.Exceptions
{
    public class ContentCanNotBeNullOrEmpty : InfrastructureException
    {
        public ContentCanNotBeNullOrEmpty() : base("Content Can not be null or empty") { }
    }
}
