using OrderManagementService.Domain.Entities.Base.BusinessValidation;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.Domain.Entities.Rules
{
    public class StringNotNullOrEmptyValidation : DomainValidation<string, StringCannotBeNullOrEmptyException>
    {
        public StringNotNullOrEmptyValidation(string value) : base(value) { }
        protected override bool IsValid() => !string.IsNullOrWhiteSpace(Value);
    }
}
