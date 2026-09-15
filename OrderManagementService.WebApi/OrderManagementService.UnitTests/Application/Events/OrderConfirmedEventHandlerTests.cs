using FluentAssertions;
using Moq;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Events.OrderConfirmed;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Events;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Events
{
    public class OrderConfirmedEventHandlerTests
    {
        private readonly Mock<IProductCommandRepository> _productRepositoryStub = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly OrderConfirmedEventHandler _sut;

        public OrderConfirmedEventHandlerTests()
        {
            _sut = new OrderConfirmedEventHandler(_productRepositoryStub.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Decreases_Stock_And_Commits_When_Order_Confirmed()
        {
            // Arrange
            var product = ProductMother.Laptop(stock: 10);
            var order = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), product.Id, price: 100m, qty: 4);
            var notification = new OrderConfirmedEvent(order.Id, order.Items.ToList().AsReadOnly());

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            _unitOfWorkMock
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 1 });

            // Act
            await _sut.Handle(notification, CancellationToken.None);

            // Assert
            product.Stock.Value.Should().Be(6); // 10 - 4 = 6
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
