using FluentAssertions;
using Moq;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.Domain.Services;
using OrderManagementService.Domain.Services.DomainServices.OrderConfirmation;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Domain.Services
{
    public class OrderConfirmationDomainServiceTests
    {
        private readonly Mock<IOrderInventoryChecker> _inventoryCheckerStub = new();

        [Fact]
        public async Task ConfirmAsync_Confirms_Order_When_Inventory_Is_Sufficient()
        {
            // Arrange
            var order = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 2);
            _inventoryCheckerStub
                .Setup(c => c.HasSufficientStockAsync(order.Items, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = new OrderConfirmationDomainService(_inventoryCheckerStub.Object);

            // Act
            await sut.ConfirmAsync(order);

            // Assert
            order.State.Should().BeOfType<ConfirmedState>();
        }

        [Fact]
        public async Task ConfirmAsync_Throws_OrderMustHaveItemsException_When_Order_Has_No_Items()
        {
            // Arrange
            var order = OrderMother.EmptyPendingOrder();
            var sut = new OrderConfirmationDomainService(_inventoryCheckerStub.Object);

            // Act
            var act = () => sut.ConfirmAsync(order);

            // Assert
            await act.Should().ThrowAsync<OrderMustHaveItemsException>();
        }

        [Fact]
        public async Task ConfirmAsync_Throws_InsufficientStockException_When_Inventory_Is_Insufficient()
        {
            // Arrange
            var order = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 2);
            _inventoryCheckerStub
                .Setup(c => c.HasSufficientStockAsync(order.Items, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var sut = new OrderConfirmationDomainService(_inventoryCheckerStub.Object);

            // Act
            var act = () => sut.ConfirmAsync(order);

            // Assert
            await act.Should().ThrowAsync<InsufficientStockException>();
            order.State.Should().BeOfType<PendingState>();
        }
    }
}
