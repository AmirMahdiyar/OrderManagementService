using FluentAssertions;
using OrderManagementService.Application.Commands.BulkInsertOrders;
using OrderManagementService.Application.Dtos;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class BulkInsertOrdersCommandValidatorTests
    {
        private readonly BulkInsertOrdersCommandValidator _sut = new();

        [Fact]
        public void Validate_Fails_When_Orders_List_Is_Empty()
        {
            // Arrange
            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>()
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(BulkInsertOrdersCommand.Orders));
        }

        [Fact]
        public void Validate_Fails_When_An_Order_Has_Empty_CustomerId()
        {
            // Arrange
            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>
                {
                    new(Guid.Empty, new List<OrderItemDto> { new(Guid.NewGuid(), 1) })
                }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("CustomerId"));
        }

        [Fact]
        public void Validate_Succeeds_When_All_Orders_Are_Valid()
        {
            // Arrange
            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>
                {
                    new(Guid.NewGuid(), new List<OrderItemDto> { new(Guid.NewGuid(), 2) })
                }
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
