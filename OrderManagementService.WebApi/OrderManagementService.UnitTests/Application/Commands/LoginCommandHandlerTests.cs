using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.Login;
using OrderManagementService.Application.Contracts.Jwt;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Services;
using OrderManagementService.Domain.Services.DomainServices.UserPassword;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserQueryRepository> _userRepositoryStub = new();
        private readonly Mock<IUserPasswordDomainService> _userPasswordDomainServiceStub = new();
        private readonly Mock<IJwtProvider> _jwtProviderStub = new();
        private readonly Mock<IPasswordHasher> _passwordHasherStub = new();

        private readonly LoginCommandHandler _sut;

        public LoginCommandHandlerTests()
        {
            _passwordHasherStub.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_pwd");

            _sut = new LoginCommandHandler(
                _userRepositoryStub.Object,
                _userPasswordDomainServiceStub.Object,
                _jwtProviderStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_User_Not_Found()
        {
            // Arrange
            var command = new LoginCommand
            {
                Username = "nonexistent",
                Password = "Password123"
            };

            _userRepositoryStub
                .Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Invalid username or password*");
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Password_Verification_Fails()
        {
            // Arrange
            var user = User.Create("existingUser", "Secret123", "User", _passwordHasherStub.Object);
            var command = new LoginCommand
            {
                Username = "existingUser",
                Password = "WrongPassword"
            };

            _userRepositoryStub
                .Setup(r => r.GetByUsernameAsync("existingUser", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userPasswordDomainServiceStub
                .Setup(s => s.VerifyPassword(user, "WrongPassword"))
                .Returns(false);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Invalid username or password*");
        }

        [Fact]
        public async Task Handle_Returns_Token_When_Credentials_Are_Valid()
        {
            // Arrange
            var user = User.Create("existingUser", "Secret123", "User", _passwordHasherStub.Object);
            var command = new LoginCommand
            {
                Username = "existingUser",
                Password = "Secret123"
            };

            _userRepositoryStub
                .Setup(r => r.GetByUsernameAsync("existingUser", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userPasswordDomainServiceStub
                .Setup(s => s.VerifyPassword(user, "Secret123"))
                .Returns(true);

            _jwtProviderStub
                .Setup(j => j.GenerateToken(user))
                .Returns("mock-jwt-token-xyz");

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.Token.Should().Be("mock-jwt-token-xyz");
        }
    }
}
