using OrderManagementService.Domain.Entities.Rules;

namespace OrderManagementService.Domain.Entities.ValueObjects
{
    public readonly record struct Quantity
    {
        public int Value { get; }

        private Quantity(int value)
        {
            new QuantityGreaterThanZeroValidation(value).Validate();
            Value = value;
        }

        public static Quantity Create(int value) => new Quantity(value);
    }
}
