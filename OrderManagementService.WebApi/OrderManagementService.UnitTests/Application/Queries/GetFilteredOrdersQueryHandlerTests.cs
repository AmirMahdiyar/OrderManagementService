using FluentAssertions;
using Moq;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Application.Queries.GetFilteredOrders;
using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Queries
{
    public class GetFilteredOrdersQueryHandlerTests
    {
        private readonly Mock<IOrderQueryRepository> _queryRepositoryStub = new();
        private readonly GetFilteredOrdersQueryHandler _sut;

        public GetFilteredOrdersQueryHandlerTests()
        {
            _sut = new GetFilteredOrdersQueryHandler(_queryRepositoryStub.Object);
        }

        [Theory]
        [InlineData("Cancelled")]
        [InlineData("Unknown")]
        [InlineData("InvalidStatus")]
        [InlineData("123")]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_OrderStatus_Is_Invalid(string invalidStatus)
        {
            // Arrange
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 10 },
                OrderStatus = invalidStatus
            };

            // Act
            var act = () => _sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Order status does not exist*");
        }

        [Fact]
        public async Task Handle_Invokes_Repository_With_Normalized_Status_And_Returns_Mapped_Pagination_Result()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var order = OrderMother.ConfirmedOrder(customerId, Guid.NewGuid(), 100m, 1);
            var paginatedOrders = new PaginationDto<Order>(
                Items: new List<Order> { order },
                TotalCount: 1,
                CurrentPage: 1,
                TotalPages: 1,
                HasNextPage: false,
                HasPreviousPage: false);

            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 10 },
                CustomerId = customerId,
                OrderStatus = "confirmed" // lowercase -> normalized to "Confirmed"
            };

            _queryRepositoryStub
                .Setup(r => r.GetFilteredOrdersAsync(
                    It.IsAny<PaginationQuery>(),
                    customerId,
                    "Confirmed", // normalized PascalCase
                    null,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedOrders);

            // Act
            var result = await _sut.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(1);
            result.Items.Should().HaveCount(1);
            result.Items.First().CustomerId.Should().Be(customerId);
            result.Items.First().Status.Should().Be("Confirmed");
            result.Items.First().TotalAmount.Should().Be(100m);
        }
    }
}
