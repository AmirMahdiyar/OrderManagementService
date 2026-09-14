using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.States.Exceptions
{
    public class InvalidOrderStateException : DomainException
    {
        public InvalidOrderStateException() : base("The current order state does not allow this operation.") { }
        public InvalidOrderStateException(string message) : base(message) { }
    }
}
