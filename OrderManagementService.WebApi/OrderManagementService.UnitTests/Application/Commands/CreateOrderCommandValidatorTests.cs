using FluentAssertions;
using OrderManagementService.Application.Commands.CreateOrder;
using OrderManagementService.Application.Dtos;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class CreateOrderCommandValidatorTests
    {
        private readonly CreateOrderCommandValidator _sut = new();

        [Fact]
        public void Validate_Fails_When_CustomerId_Is_Empty()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.Empty,
                Items = new List<OrderItemDto> { new(Guid.NewGuid(), 1) }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.CustomerId));
        }

        [Fact]
        public void Validate_Fails_When_Items_List_Is_Empty()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto>()
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.Items));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Validate_Fails_When_Item_Quantity_Is_Zero_Or_Negative(int quantity)
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto> { new(Guid.NewGuid(), quantity) }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
        }

        [Fact]
        public void Validate_Fails_When_Item_ProductId_Is_Empty()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto> { new(Guid.Empty, 1) }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("ProductId"));
        }

        [Fact]
        public void Validate_Succeeds_When_Command_Has_Valid_Data()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto>
                {
                    new(Guid.NewGuid(), 2),
                    new(Guid.NewGuid(), 5)
                }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
