using OrderManagementService.Domain.Entities.Rules;

namespace OrderManagementService.Domain.Entities.ValueObjects
{
    public readonly record struct Money
    {
        public decimal Value { get; }

        private Money(decimal value)
        {
            new MoneyNotNegativeValidation(value).Validate();
            Value = value;
        }

        public static Money Create(decimal value) => new Money(value);

        public static Money operator +(Money a, Money b) => Create(a.Value + b.Value);
        public static Money operator *(Quantity a, Money b) => Create(a.Value * b.Value);
        public static Money operator *(Money a, Quantity b) => Create(a.Value * b.Value);
    }
}
