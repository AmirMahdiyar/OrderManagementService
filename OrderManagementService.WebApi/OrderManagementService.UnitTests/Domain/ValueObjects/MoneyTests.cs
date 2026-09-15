using FluentAssertions;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.UnitTests.Domain.ValueObjects
{
    public class MoneyTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        public void Create_Succeeds_With_Positive_Or_Zero_Amount(decimal amount)
        {
            // Arrange & Act
            var sut = Money.Create(amount);

            // Assert
            sut.Value.Should().Be(amount);
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Create_Throws_MoneyCannotBeNegativeException_With_Negative_Amount(decimal amount)
        {
            // Arrange & Act
            var act = () => Money.Create(amount);

            // Assert
            act.Should().Throw<MoneyCannotBeNegativeException>();
        }

        [Fact]
        public void Equals_Returns_True_For_Same_Value_And_False_For_Different_Value()
        {
            // Arrange
            var sut = Money.Create(50.00m);
            var identical = Money.Create(50.00m);
            var different = Money.Create(75.00m);

            // Act & Assert
            sut.Should().Be(identical);
            (sut == identical).Should().BeTrue();
            (sut != different).Should().BeTrue();
            sut.Equals(different).Should().BeFalse();
        }

        [Fact]
        public void Add_Operator_Returns_Correct_Sum()
        {
            // Arrange
            var sut = Money.Create(50.25m);
            var other = Money.Create(49.75m);

            // Act
            var result = sut + other;

            // Assert
            result.Value.Should().Be(100.00m);
        }

        [Fact]
        public void Multiply_Operator_By_Quantity_Returns_Correct_Total()
        {
            // Arrange
            var sut = Money.Create(25.50m);
            var quantity = Quantity.Create(4);

            // Act
            var result1 = sut * quantity;
            var result2 = quantity * sut;

            // Assert
            result1.Value.Should().Be(102.00m);
            result2.Value.Should().Be(102.00m);
        }
    }
}
