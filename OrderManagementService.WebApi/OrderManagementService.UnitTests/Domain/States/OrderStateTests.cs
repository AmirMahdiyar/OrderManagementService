using FluentAssertions;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Entities.States.Base;

namespace OrderManagementService.UnitTests.Domain.States
{
    public class OrderStateTests
    {
        [Theory]
        [InlineData("Pending")]
        [InlineData("pending")]
        [InlineData("PENDING")]
        [InlineData("Confirmed")]
        [InlineData("confirmed")]
        [InlineData("Shipped")]
        [InlineData("shipped")]
        [InlineData("Delivered")]
        [InlineData("delivered")]
        public void IsValid_Returns_True_For_Valid_State_Names_Case_Insensitive(string validState)
        {
            // Arrange & Act
            var result = OrderState.IsValid(validState);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("Cancelled")]
        [InlineData("Unknown")]
        [InlineData("123")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void IsValid_Returns_False_For_Invalid_Or_Empty_State_Names(string? invalidState)
        {
            // Arrange & Act
            var result = OrderState.IsValid(invalidState);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("pending", "Pending")]
        [InlineData("CONFIRMED", "Confirmed")]
        [InlineData("Shipped", "Shipped")]
        [InlineData("delivered", "Delivered")]
        [InlineData("invalid", null)]
        [InlineData(null, null)]
        public void Normalize_Returns_PascalCase_Valid_Name_Or_Null(string? input, string? expected)
        {
            // Arrange & Act
            var result = OrderState.Normalize(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Equals_Returns_True_For_Same_State_Name_Instances()
        {
            // Arrange
            var state1 = new PendingState();
            var state2 = new PendingState();

            // Act & Assert
            (state1 == state2).Should().BeTrue();
            state1.Equals(state2).Should().BeTrue();
        }

        [Fact]
        public void Equals_Returns_False_For_Different_State_Name_Instances()
        {
            // Arrange
            var state1 = new PendingState();
            var state2 = new ConfirmedState();

            // Act & Assert
            (state1 == state2).Should().BeFalse();
            (state1 != state2).Should().BeTrue();
            state1.Equals(state2).Should().BeFalse();
        }
    }
}
