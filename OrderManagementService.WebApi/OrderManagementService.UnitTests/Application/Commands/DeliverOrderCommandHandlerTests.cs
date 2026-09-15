using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.DeliverOrder;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.States;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class DeliverOrderCommandHandlerTests
    {
        private readonly Mock<IOrderCommandRepository> _orderRepositoryStub = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkStub = new();

        private readonly DeliverOrderCommandHandler _sut;

        public DeliverOrderCommandHandlerTests()
        {
            _sut = new DeliverOrderCommandHandler(_orderRepositoryStub.Object, _unitOfWorkStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Order_Not_Found()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new DeliverOrderCommand { OrderId = orderId };

            _orderRepositoryStub
                .Setup(r => r.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Order not found*");
        }

        [Fact]
        public async Task Handle_Transitions_Order_To_Delivered_And_Commits_Transaction()
        {
            // Arrange
            var order = OrderMother.ShippedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            var command = new DeliverOrderCommand { OrderId = order.Id };

            _orderRepositoryStub
                .Setup(r => r.GetByIdWithItemsAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _unitOfWorkStub
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 1 });

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            order.State.Should().BeOfType<DeliveredState>();
            _unitOfWorkStub.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
