using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.UnitTests.Common.Helpers;

namespace OrderManagementService.UnitTests.Common.Builders
{
    public class ProductBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Default Test Product";
        private Quantity _stock = Quantity.Create(10);
        private Money _price = Money.Create(100.00m);

        public ProductBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ProductBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ProductBuilder WithStock(int stock)
        {
            _stock = Quantity.Create(stock);
            return this;
        }

        public ProductBuilder WithStock(Quantity stock)
        {
            _stock = stock;
            return this;
        }

        public ProductBuilder WithPrice(decimal price)
        {
            _price = Money.Create(price);
            return this;
        }

        public ProductBuilder WithPrice(Money price)
        {
            _price = price;
            return this;
        }

        public Product Build()
        {
            var product = Product.Create(_name, _stock, _price);
            product.WithId(_id);
            return product;
        }
    }
}
