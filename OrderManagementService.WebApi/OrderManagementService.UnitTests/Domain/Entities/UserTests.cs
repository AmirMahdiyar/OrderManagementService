using FluentAssertions;
using Moq;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Services;

namespace OrderManagementService.UnitTests.Domain.Entities
{
    public class UserTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherStub = new();

        public UserTests()
        {
            _passwordHasherStub
                .Setup(h => h.Hash(It.IsAny<string>()))
                .Returns((string raw) => $"hashed_{raw}");
        }

        [Fact]
        public void Create_Succeeds_With_Valid_Parameters()
        {
            // Arrange
            var username = "testuser";
            var password = "SecurePassword123";
            var role = "Admin";

            // Act
            var sut = User.Create(username, password, role, _passwordHasherStub.Object);

            // Assert
            sut.Username.Should().Be(username);
            sut.PasswordHash.Should().Be("hashed_SecurePassword123");
            sut.Role.Should().Be(role);
            sut.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Throws_StringCannotBeNullOrEmptyException_When_Username_Is_Null_Or_Empty(string? invalidUsername)
        {
            // Arrange & Act
            var act = () => User.Create(invalidUsername!, "ValidPassword123", "User", _passwordHasherStub.Object);

            // Assert
            act.Should().Throw<StringCannotBeNullOrEmptyException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Throws_StringCannotBeNullOrEmptyException_When_Password_Is_Null_Or_Empty(string? invalidPassword)
        {
            // Arrange & Act
            var act = () => User.Create("validUser", invalidPassword!, "User", _passwordHasherStub.Object);

            // Assert
            act.Should().Throw<StringCannotBeNullOrEmptyException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_Throws_StringCannotBeNullOrEmptyException_When_Role_Is_Null_Or_Empty(string? invalidRole)
        {
            // Arrange & Act
            var act = () => User.Create("validUser", "ValidPassword123", invalidRole!, _passwordHasherStub.Object);

            // Assert
            act.Should().Throw<StringCannotBeNullOrEmptyException>();
        }
    }
}
