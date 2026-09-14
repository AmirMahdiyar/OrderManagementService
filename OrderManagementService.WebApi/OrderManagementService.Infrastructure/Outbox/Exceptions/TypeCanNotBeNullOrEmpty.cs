using OrderManagementService.Domain.Exceptions.Infrastructure;

namespace OrderManagementService.Infrastructure.Outbox.Exceptions
{
    public class TypeCanNotBeNullOrEmpty : InfrastructureException
    {
        public TypeCanNotBeNullOrEmpty() : base("Type Can not be null or empty") { }
    }
}
