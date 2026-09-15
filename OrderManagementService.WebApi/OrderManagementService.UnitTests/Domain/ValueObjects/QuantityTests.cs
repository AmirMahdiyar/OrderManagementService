using FluentAssertions;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.ValueObjects;

namespace OrderManagementService.UnitTests.Domain.ValueObjects
{
    public class QuantityTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(10000)]
        public void Create_Succeeds_With_Positive_Integer(int amount)
        {
            // Arrange & Act
            var sut = Quantity.Create(amount);

            // Assert
            sut.Value.Should().Be(amount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_Throws_QuantityMustBeGreaterThanZeroException_With_Zero_Or_Negative(int amount)
        {
            // Arrange & Act
            var act = () => Quantity.Create(amount);

            // Assert
            act.Should().Throw<QuantityMustBeGreaterThanZeroException>();
        }

        [Fact]
        public void Equals_Returns_True_For_Identical_Quantities()
        {
            // Arrange
            var sut = Quantity.Create(10);
            var identical = Quantity.Create(10);
            var different = Quantity.Create(20);

            // Act & Assert
            sut.Should().Be(identical);
            (sut == identical).Should().BeTrue();
            (sut != different).Should().BeTrue();
            sut.Equals(different).Should().BeFalse();
        }
    }
}
