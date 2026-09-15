using FluentAssertions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Rules.Exceptions;

namespace OrderManagementService.UnitTests.Domain.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Create_Succeeds_With_Valid_Parameters()
        {
            // Arrange
            var fullName = "Alice Smith";
            var userId = Guid.NewGuid();

            // Act
            var sut = Customer.Create(fullName, userId);

            // Assert
            sut.FullName.Should().Be(fullName);
            sut.UserId.Should().Be(userId);
            sut.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Throws_StringCannotBeNullOrEmptyException_When_FullName_Is_Null_Or_Empty(string? invalidName)
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var act = () => Customer.Create(invalidName!, userId);

            // Assert
            act.Should().Throw<StringCannotBeNullOrEmptyException>();
        }
    }
}
