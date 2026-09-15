using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagementService.Application.Commands.Login;
using OrderManagementService.Application.PipelineBehaviors;

namespace OrderManagementService.UnitTests.Application.PipelineBehaviors
{
    public class LoggingBehaviorTests
    {
        private readonly Mock<ILogger<LoggingBehavior<LoginCommand, LoginCommandResponse>>> _loginLoggerMock = new();

        [Fact]
        public async Task Handle_Masks_Password_When_Request_Is_LoginCommand()
        {
            // Arrange
            var sut = new LoggingBehavior<LoginCommand, LoginCommandResponse>(_loginLoggerMock.Object);
            var command = new LoginCommand
            {
                Username = "admin",
                Password = "SuperSecretPassword"
            };
            var expectedResponse = LoginCommandResponse.Response().WithToken("token123");
            RequestHandlerDelegate<LoginCommandResponse> next = (ct) => Task.FromResult(expectedResponse);

            // Act
            var response = await sut.Handle(command, next, CancellationToken.None);

            // Assert
            response.Token.Should().Be("token123");
            _loginLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("***MASKED***") && !v.ToString()!.Contains("SuperSecretPassword")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_Logs_Error_And_Rethrows_When_Next_Delegate_Throws()
        {
            // Arrange
            var sut = new LoggingBehavior<LoginCommand, LoginCommandResponse>(_loginLoggerMock.Object);
            var command = new LoginCommand
            {
                Username = "admin",
                Password = "pass"
            };
            RequestHandlerDelegate<LoginCommandResponse> next = (ct) => Task.FromException<LoginCommandResponse>(new InvalidOperationException("Handler exploded"));

            // Act
            var act = () => sut.Handle(command, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Handler exploded");
            _loginLoggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed handling LoginCommand")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
