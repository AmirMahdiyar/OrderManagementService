using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.ConfirmOrder;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Services.DomainServices.OrderConfirmation;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class ConfirmOrderCommandHandlerTests
    {
        private readonly Mock<IOrderCommandRepository> _orderRepositoryStub = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkStub = new();
        private readonly Mock<IOrderConfirmationDomainService> _domainServiceMock = new();

        private readonly ConfirmOrderCommandHandler _sut;

        public ConfirmOrderCommandHandlerTests()
        {
            _sut = new ConfirmOrderCommandHandler(
                _orderRepositoryStub.Object,
                _unitOfWorkStub.Object,
                _domainServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Order_Not_Found()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new ConfirmOrderCommand { OrderId = orderId };

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
        public async Task Handle_Invokes_Confirmation_Domain_Service_And_Commits_Transaction()
        {
            // Arrange
            var order = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            var command = new ConfirmOrderCommand { OrderId = order.Id };

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
            _domainServiceMock.Verify(s => s.ConfirmAsync(order, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkStub.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
