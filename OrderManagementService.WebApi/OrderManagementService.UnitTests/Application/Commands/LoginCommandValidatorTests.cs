using FluentAssertions;
using OrderManagementService.Application.Commands.Login;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _sut = new();

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_Fails_When_Username_Is_Null_Or_Empty(string? username)
        {
            // Arrange
            var command = new LoginCommand
            {
                Username = username!,
                Password = "Password123"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Username));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_Fails_When_Password_Is_Null_Or_Empty(string? password)
        {
            // Arrange
            var command = new LoginCommand
            {
                Username = "validUser",
                Password = password!
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Password));
        }

        [Fact]
        public void Validate_Succeeds_When_Username_And_Password_Are_Valid()
        {
            // Arrange
            var command = new LoginCommand
            {
                Username = "validUser",
                Password = "ValidPass123"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
