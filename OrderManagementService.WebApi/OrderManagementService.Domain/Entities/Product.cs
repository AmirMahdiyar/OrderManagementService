using OrderManagementService.Domain.Entities.Base.Entity;
using OrderManagementService.Domain.Entities.Rules;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.Domain.Entities
{
    public class Product : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public Quantity Stock { get; private set; }
        public Money Price { get; private set; }

        protected Product() { } //For EF

        protected Product(string name, Quantity stock, Money price)
        {
            new StringNotNullOrEmptyValidation(name).Validate();

            Id = InitialId();
            Name = name;
            Stock = stock;
            Price = price;
        }

        public static Product Create(string name, Quantity stock, Money price)
            => new Product(name, stock, price);

        public void DecreaseStock(Quantity amount)
        {
            new StockMustBeSufficientValidation(Stock, amount).Validate();
            Stock = Quantity.Create(Stock.Value - amount.Value);
        }

        protected override Guid InitialId()
            => Guid.NewGuid();
    }
}
