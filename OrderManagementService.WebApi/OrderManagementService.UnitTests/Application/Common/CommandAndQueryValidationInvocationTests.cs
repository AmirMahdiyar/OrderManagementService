using FluentAssertions;
using OrderManagementService.Application.Commands.BulkInsertOrders;
using OrderManagementService.Application.Commands.ConfirmOrder;
using OrderManagementService.Application.Commands.CreateOrder;
using OrderManagementService.Application.Commands.DeleteOrder;
using OrderManagementService.Application.Commands.DeliverOrder;
using OrderManagementService.Application.Commands.Login;
using OrderManagementService.Application.Commands.ShipOrder;
using OrderManagementService.Application.Common;
using OrderManagementService.Application.Dtos;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Application.Queries.GetFilteredOrders;
using OrderManagementService.Application.Queries.GetOrderById;

namespace OrderManagementService.UnitTests.Application.Common
{
    public class CommandAndQueryValidationInvocationTests
    {
        [Fact]
        public void CreateOrderCommand_Validate_Throws_When_Invalid()
        {
            var command = new CreateOrderCommand { CustomerId = Guid.Empty };
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void CreateOrderCommand_Validate_Succeeds_When_Valid()
        {
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items = new List<OrderItemDto> { new(Guid.NewGuid(), 1) }
            };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void BulkInsertOrdersCommand_Validate_Throws_When_Invalid()
        {
            var command = new BulkInsertOrdersCommand();
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void ConfirmOrderCommand_Validate_Throws_When_Invalid()
        {
            var command = new ConfirmOrderCommand { OrderId = Guid.Empty };
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void ConfirmOrderCommand_Validate_Succeeds_When_Valid()
        {
            var command = new ConfirmOrderCommand { OrderId = Guid.NewGuid() };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void ShipOrderCommand_Validate_Throws_When_Invalid()
        {
            var command = new ShipOrderCommand { OrderId = Guid.Empty };
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void ShipOrderCommand_Validate_Succeeds_When_Valid()
        {
            var command = new ShipOrderCommand { OrderId = Guid.NewGuid() };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void DeliverOrderCommand_Validate_Throws_When_Invalid()
        {
            var command = new DeliverOrderCommand { OrderId = Guid.Empty };
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void DeliverOrderCommand_Validate_Succeeds_When_Valid()
        {
            var command = new DeliverOrderCommand { OrderId = Guid.NewGuid() };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void DeleteOrderCommand_Validate_Throws_When_Invalid()
        {
            var command = new DeleteOrderCommand { OrderId = Guid.Empty };
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void DeleteOrderCommand_Validate_Succeeds_When_Valid()
        {
            var command = new DeleteOrderCommand { OrderId = Guid.NewGuid() };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void LoginCommand_Validate_Throws_When_Invalid()
        {
            var command = new LoginCommand();
            var act = () => command.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void LoginCommand_Validate_Succeeds_When_Valid()
        {
            var command = new LoginCommand { Username = "user", Password = "password" };
            var act = () => command.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void GetOrderByIdQuery_Validate_Throws_When_Invalid()
        {
            var query = new GetOrderByIdQuery { OrderId = Guid.Empty };
            var act = () => query.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void GetOrderByIdQuery_Validate_Succeeds_When_Valid()
        {
            var query = new GetOrderByIdQuery { OrderId = Guid.NewGuid() };
            var act = () => query.Validate();
            act.Should().NotThrow();
        }

        [Fact]
        public void GetFilteredOrdersQuery_Validate_Throws_When_Invalid()
        {
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 0, Size = 0 }
            };
            var act = () => query.Validate();
            act.Should().Throw<InputValidationFailedApplicationException>();
        }

        [Fact]
        public void GetFilteredOrdersQuery_Validate_Succeeds_When_Valid()
        {
            var query = new GetFilteredOrdersQuery
            {
                Pagination = new PaginationQuery { Page = 1, Size = 10 }
            };
            var act = () => query.Validate();
            act.Should().NotThrow();
        }
    }
}
