using FluentAssertions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Domain.Entities
{
    public class ProductTests
    {
        [Fact]
        public void Create_Succeeds_With_Valid_Parameters()
        {
            // Arrange
            var name = "Gaming Laptop";
            var stock = Quantity.Create(15);
            var price = Money.Create(1500.00m);

            // Act
            var sut = Product.Create(name, stock, price);

            // Assert
            sut.Name.Should().Be(name);
            sut.Stock.Should().Be(stock);
            sut.Price.Should().Be(price);
            sut.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Throws_StringCannotBeNullOrEmptyException_When_Name_Is_Null_Or_Empty(string? invalidName)
        {
            // Arrange
            var stock = Quantity.Create(10);
            var price = Money.Create(100.00m);

            // Act
            var act = () => Product.Create(invalidName!, stock, price);

            // Assert
            act.Should().Throw<StringCannotBeNullOrEmptyException>();
        }

        [Fact]
        public void IncreaseStock_Increases_Available_Stock_By_Given_Quantity()
        {
            // Arrange
            var sut = ProductMother.Laptop(stock: 10);
            var amountToAdd = Quantity.Create(5);

            // Act
            sut.IncreaseStock(amountToAdd);

            // Assert
            sut.Stock.Value.Should().Be(15);
        }

        [Fact]
        public void DecreaseStock_Decreases_Stock_When_Sufficient()
        {
            // Arrange
            var sut = ProductMother.Laptop(stock: 10);
            var amountToDeduct = Quantity.Create(4);

            // Act
            sut.DecreaseStock(amountToDeduct);

            // Assert
            sut.Stock.Value.Should().Be(6);
        }

        [Fact]
        public void DecreaseStock_Throws_InsufficientStockException_When_Requested_Quantity_Exceeds_Stock()
        {
            // Arrange
            var sut = ProductMother.Laptop(stock: 5);
            var excessiveAmount = Quantity.Create(6);

            // Act
            var act = () => sut.DecreaseStock(excessiveAmount);

            // Assert
            act.Should().Throw<InsufficientStockException>();
        }
    }
}
