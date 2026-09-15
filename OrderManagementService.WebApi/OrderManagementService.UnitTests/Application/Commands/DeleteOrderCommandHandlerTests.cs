using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.DeleteOrder;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.States.Exceptions;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class DeleteOrderCommandHandlerTests
    {
        private readonly Mock<IOrderCommandRepository> _orderRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkStub = new();

        private readonly DeleteOrderCommandHandler _sut;

        public DeleteOrderCommandHandlerTests()
        {
            _sut = new DeleteOrderCommandHandler(_orderRepositoryMock.Object, _unitOfWorkStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Order_Not_Found()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new DeleteOrderCommand { OrderId = orderId };

            _orderRepositoryMock
                .Setup(r => r.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Order not found*");
        }

        [Fact]
        public async Task Handle_Throws_InvalidOrderStateException_When_Order_Is_Shipped()
        {
            // Arrange
            var order = OrderMother.ShippedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            var command = new DeleteOrderCommand { OrderId = order.Id };

            _orderRepositoryMock
                .Setup(r => r.GetByIdWithItemsAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOrderStateException>();
        }

        [Fact]
        public async Task Handle_Executes_Order_Delete_And_Deletes_From_Repository_And_Commits()
        {
            // Arrange
            var order = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            var command = new DeleteOrderCommand { OrderId = order.Id };

            _orderRepositoryMock
                .Setup(r => r.GetByIdWithItemsAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _unitOfWorkStub
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 1 });

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            _orderRepositoryMock.Verify(r => r.Delete(order), Times.Once);
            _unitOfWorkStub.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
