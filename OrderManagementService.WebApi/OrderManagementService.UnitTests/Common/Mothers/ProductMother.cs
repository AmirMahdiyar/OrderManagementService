using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Builders;

namespace OrderManagementService.UnitTests.Common.Mothers
{
    public static class ProductMother
    {
        public static Product Laptop(int stock = 10, decimal price = 1200m)
            => new ProductBuilder()
                .WithName("High-End Laptop")
                .WithStock(stock)
                .WithPrice(price)
                .Build();

        public static Product Smartphone(int stock = 20, decimal price = 800m)
            => new ProductBuilder()
                .WithName("Smartphone Pro")
                .WithStock(stock)
                .WithPrice(price)
                .Build();

        public static Product OutOfStockItem()
            => new ProductBuilder()
                .WithName("Out of Stock Gadget")
                .WithStock(0)
                .WithPrice(50m)
                .Build();

        public static Product WithSpecificId(Guid id, int stock = 10, decimal price = 100m)
            => new ProductBuilder()
                .WithId(id)
                .WithName($"Product-{id.ToString()[..8]}")
                .WithStock(stock)
                .WithPrice(price)
                .Build();
    }
}
