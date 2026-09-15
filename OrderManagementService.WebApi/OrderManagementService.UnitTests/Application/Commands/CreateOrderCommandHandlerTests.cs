using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.CreateOrder;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Dtos;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<ICustomerCommandRepository> _customerRepositoryStub = new();
        private readonly Mock<IProductCommandRepository> _productRepositoryStub = new();
        private readonly Mock<IOrderCommandRepository> _orderRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkStub = new();

        private readonly CreateOrderCommandHandler _sut;

        public CreateOrderCommandHandlerTests()
        {
            _sut = new CreateOrderCommandHandler(
                _customerRepositoryStub.Object,
                _unitOfWorkStub.Object,
                _orderRepositoryMock.Object,
                _productRepositoryStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Customer_Does_Not_Exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = new List<OrderItemDto> { new(Guid.NewGuid(), 2) }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Customer not found*");
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_One_Or_More_Products_Do_Not_Exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = new List<OrderItemDto> { new(productId, 2) }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>()); // Empty list -> not found

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*One or more products do not exist*");
        }

        [Fact]
        public async Task Handle_Throws_NoChangesApplicationException_When_Commit_Returns_Zero_Changes()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var product = ProductMother.Laptop(stock: 10, price: 1000m);
            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = new List<OrderItemDto> { new(product.Id, 1) }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            _unitOfWorkStub
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 0 });

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NoChangesApplicationException>();
        }

        [Fact]
        public async Task Handle_Creates_Order_Using_Authoritative_Product_Catalog_Price_And_Persists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var product = ProductMother.Laptop(stock: 10, price: 1200m);
            var command = new CreateOrderCommand
            {
                CustomerId = customerId,
                Items = new List<OrderItemDto> { new(product.Id, 3) }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            _unitOfWorkStub
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 1 });

            Order? savedOrder = null;
            _orderRepositoryMock
                .Setup(r => r.Add(It.IsAny<Order>()))
                .Callback<Order>(o => savedOrder = o);

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.OrderId.Should().NotBeEmpty();

            _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            savedOrder.Should().NotBeNull();
            savedOrder!.CustomerId.Should().Be(customerId);
            savedOrder.Items.Should().HaveCount(1);
            savedOrder.Items.First().UnitPrice.Value.Should().Be(1200m);
            savedOrder.TotalAmount.Value.Should().Be(3600m);
        }
    }
}
