using FluentAssertions;
using MediatR;
using Moq;
using OrderManagementService.Application.Base;
using OrderManagementService.Application.PipelineBehaviors;

namespace OrderManagementService.UnitTests.Application.PipelineBehaviors
{
    public class ValidationBehaviorTests
    {
        public interface ITestValidatableCommand : IValidatableRequest
        {
        }

        public class NonValidatableCommand
        {
        }

        [Fact]
        public async Task Handle_Calls_Validate_When_Request_Implements_IValidatableRequest()
        {
            // Arrange
            var sut = new ValidationBehavior<ITestValidatableCommand, string>();
            var mockCommand = new Mock<ITestValidatableCommand>();
            mockCommand.Setup(c => c.Validate());
            RequestHandlerDelegate<string> next = (ct) => Task.FromResult("success");

            // Act
            var result = await sut.Handle(mockCommand.Object, next, CancellationToken.None);

            // Assert
            result.Should().Be("success");
            mockCommand.Verify(c => c.Validate(), Times.Once);
        }

        [Fact]
        public async Task Handle_Proceeds_Directly_When_Request_Does_Not_Implement_IValidatableRequest()
        {
            // Arrange
            var sut = new ValidationBehavior<NonValidatableCommand, string>();
            var command = new NonValidatableCommand();
            RequestHandlerDelegate<string> next = (ct) => Task.FromResult("passed");

            // Act
            var result = await sut.Handle(command, next, CancellationToken.None);

            // Assert
            result.Should().Be("passed");
        }
    }
}
