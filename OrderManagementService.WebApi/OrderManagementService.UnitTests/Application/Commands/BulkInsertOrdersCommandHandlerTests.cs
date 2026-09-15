using FluentAssertions;
using Moq;
using OrderManagementService.Application.Commands.BulkInsertOrders;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.Dtos;
using OrderManagementService.Application.Exceptions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Application.Commands
{
    public class BulkInsertOrdersCommandHandlerTests
    {
        private readonly Mock<ICustomerCommandRepository> _customerRepositoryStub = new();
        private readonly Mock<IProductCommandRepository> _productRepositoryStub = new();
        private readonly Mock<IOrderCommandRepository> _orderRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkStub = new();

        private readonly BulkInsertOrdersCommandHandler _sut;

        public BulkInsertOrdersCommandHandlerTests()
        {
            _sut = new BulkInsertOrdersCommandHandler(
                _customerRepositoryStub.Object,
                _orderRepositoryMock.Object,
                _productRepositoryStub.Object,
                _unitOfWorkStub.Object);
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Any_Customer_Does_Not_Exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>
                {
                    new(customerId, new List<OrderItemDto> { new(Guid.NewGuid(), 1) })
                }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Customers do not exist*");
        }

        [Fact]
        public async Task Handle_Throws_InputValidationFailedApplicationException_When_Any_Product_Does_Not_Exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>
                {
                    new(customerId, new List<OrderItemDto> { new(productId, 1) })
                }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>()); // empty -> missing

            // Act
            var act = () => _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InputValidationFailedApplicationException>()
                .WithMessage("*Products do not exist*");
        }

        [Fact]
        public async Task Handle_Creates_All_Orders_With_Catalog_Prices_And_Persists_Batch()
        {
            // Arrange
            var customer1 = CustomerMother.JohnDoe();
            var customer2 = CustomerMother.JaneSmith();
            var laptop = ProductMother.Laptop(stock: 10, price: 1500m);
            var phone = ProductMother.Smartphone(stock: 20, price: 800m);

            var command = new BulkInsertOrdersCommand
            {
                Orders = new List<BulkOrderDto>
                {
                    new(customer1.Id, new List<OrderItemDto> { new(laptop.Id, 1) }),
                    new(customer2.Id, new List<OrderItemDto> { new(phone.Id, 2) })
                }
            };

            _customerRepositoryStub
                .Setup(r => r.ExistsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _productRepositoryStub
                .Setup(r => r.GetProductsByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { laptop, phone });

            _unitOfWorkStub
                .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SavingResult { ChangesCount = 2 });

            List<Order>? insertedOrders = null;
            _orderRepositoryMock
                .Setup(r => r.AddRange(It.IsAny<IEnumerable<Order>>()))
                .Callback<IEnumerable<Order>>(orders => insertedOrders = orders.ToList());

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.InsertedCount.Should().Be(2);
            _orderRepositoryMock.Verify(r => r.AddRange(It.IsAny<IEnumerable<Order>>()), Times.Once);
            insertedOrders.Should().HaveCount(2);
            insertedOrders![0].TotalAmount.Value.Should().Be(1500m);
            insertedOrders[1].TotalAmount.Value.Should().Be(1600m);
        }
    }
}
