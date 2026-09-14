using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Rules.Exceptions
{
    public class StringCannotBeNullOrEmptyException : DomainException
    {
        public StringCannotBeNullOrEmptyException() : base("Field cannot be null or empty.") { }
    }
}
