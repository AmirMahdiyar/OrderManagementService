using FluentAssertions;
using Moq;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Events.OrderDeleted;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Events;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Events
{
    public class OrderDeletedEventHandlerTests
    {
        private readonly Mock<IProductCommandRepository> _productRepositoryStub = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly OrderDeletedEventHandler _sut;

        public OrderDeletedEventHandlerTests()
        {
            _sut = new OrderDeletedEventHandler(_productRepositoryStub.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Does_Not_Restock_Or_Commit_When_Deleted_Order_Was_In_Pending_State()
        {
            // Arrange
            var product = ProductMother.Laptop(stock: 10);
            var order = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), product.Id, price: 100m, qty: 3);
            var notification = new OrderDeletedEvent(order.Id, order.Items, "Pending");

            // Act
            await _sut.Handle(notification, CancellationToken.None);

            // Assert
            _productRepositoryStub.Verify(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Restocks_Products_And_Commits_When_Deleted_Order_Was_In_Confirmed_State()
        {
            // Arrange
            var product = ProductMother.Laptop(stock: 5);
            var order = OrderMother.ConfirmedOrder(Guid.NewGuid(), product.Id, price: 100m, qty: 3);
            var notification = new OrderDeletedEvent(order.Id, order.Items, "Confirmed");

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            _unitOfWorkMock
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 1 });

            // Act
            await _sut.Handle(notification, CancellationToken.None);

            // Assert
            product.Stock.Value.Should().Be(8); // 5 + 3 = 8
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
