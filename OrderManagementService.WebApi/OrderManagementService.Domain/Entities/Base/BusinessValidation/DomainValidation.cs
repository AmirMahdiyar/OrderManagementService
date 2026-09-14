using OrderManagementService.Domain.Exceptions.DomainExceptions.Base;

namespace OrderManagementService.Domain.Entities.Base.BusinessValidation
{
    public abstract class DomainValidation<TValue, TException>
        where TException : DomainException, new()
    {
        protected DomainValidation(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }

        protected abstract bool IsValid();

        public void Validate()
        {
            if (!IsValid())
                throw new TException();
        }
    }
}
