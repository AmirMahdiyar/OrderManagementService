using FluentAssertions;
using Moq;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Services;
using OrderManagementService.Domain.Services.DomainServices.UserPassword;

namespace OrderManagementService.UnitTests.Domain.Services
{
    public class UserPasswordDomainServiceTests
    {
        private readonly Mock<IPasswordHasher> _passwordHasherStub = new();

        [Fact]
        public void VerifyPassword_Returns_True_When_Hasher_Verifies_Match()
        {
            // Arrange
            _passwordHasherStub
                .Setup(h => h.Hash("Secret123"))
                .Returns("hashed_Secret123");
            _passwordHasherStub
                .Setup(h => h.Verify("Secret123", "hashed_Secret123"))
                .Returns(true);

            var user = User.Create("admin", "Secret123", "Admin", _passwordHasherStub.Object);
            var sut = new UserPasswordDomainService(_passwordHasherStub.Object);

            // Act
            var result = sut.VerifyPassword(user, "Secret123");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_Returns_False_When_Hasher_Fails_Match()
        {
            // Arrange
            _passwordHasherStub
                .Setup(h => h.Hash("Secret123"))
                .Returns("hashed_Secret123");
            _passwordHasherStub
                .Setup(h => h.Verify("WrongPassword", "hashed_Secret123"))
                .Returns(false);

            var user = User.Create("admin", "Secret123", "Admin", _passwordHasherStub.Object);
            var sut = new UserPasswordDomainService(_passwordHasherStub.Object);

            // Act
            var result = sut.VerifyPassword(user, "WrongPassword");

            // Assert
            result.Should().BeFalse();
        }
    }
}
