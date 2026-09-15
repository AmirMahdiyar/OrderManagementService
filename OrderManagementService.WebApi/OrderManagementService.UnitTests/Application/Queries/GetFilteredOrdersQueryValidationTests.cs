using FluentAssertions;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Queries.GetFilteredOrders;

namespace OrderManagementService.UnitTests.Application.Queries
{
    public class GetFilteredOrdersQueryValidationTests
    {
        private readonly GetFilteredOrdersQueryValidation _sut = new();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_Fails_When_Page_Is_Zero_Or_Negative(int page)
        {
            // Arrange
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = page, Size = 10 }
            };

            // Act
            var result = _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Page"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Validate_Fails_When_Size_Is_Zero_Or_Negative(int size)
        {
            // Arrange
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = size }
            };

            // Act
            var result = _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("Size"));
        }

        [Fact]
        public void Validate_Fails_When_EndDate_Is_Earlier_Than_StartDate()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-1);
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 10 },
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var result = _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("EndDate"));
        }

        [Fact]
        public void Validate_Fails_When_OrderStatus_Is_Invalid()
        {
            // Arrange
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 10 },
                OrderStatus = "InvalidStatus"
            };

            // Act
            var result = _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName.Contains("OrderStatus"));
        }

        [Fact]
        public void Validate_Succeeds_When_All_Inputs_Are_Valid()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 20 },
                CustomerId = Guid.NewGuid(),
                OrderStatus = "Confirmed",
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var result = _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
