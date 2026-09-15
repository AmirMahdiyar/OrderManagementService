using FluentAssertions;
using Moq;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Application.Queries.GetOrderById;
using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderQueryRepository> _queryRepositoryStub = new();
        private readonly GetOrderByIdQueryHandler _sut;

        public GetOrderByIdQueryHandlerTests()
        {
            _sut = new GetOrderByIdQueryHandler(_queryRepositoryStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Order_Not_Found()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var query = new GetOrderByIdQuery { OrderId = orderId };

            _queryRepositoryStub
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act
            var act = () => _sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Order not found*");
        }

        [Fact]
        public async Task Handle_Returns_Mapped_Response_Dto_With_All_Order_Items()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var order = OrderMother.PendingOrderWithOneItem(customerId, productId, price: 250m, qty: 3);

            var query = new GetOrderByIdQuery { OrderId = order.Id };

            _queryRepositoryStub
                .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CustomerId.Should().Be(customerId);
            result.Status.Should().Be("Pending");
            result.TotalAmount.Should().Be(750m);
            result.Items.Should().HaveCount(1);
            result.Items.First().ProductId.Should().Be(productId);
            result.Items.First().Quantity.Should().Be(3);
            result.Items.First().UnitPrice.Should().Be(250m);
        }
    }
}
