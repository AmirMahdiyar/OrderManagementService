using FluentAssertions;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.Events;
using OrderManagementService.Domain.Entities.Rules.Exceptions;
using OrderManagementService.Domain.Entities.States;
using DomainInvalidOrderStateException = OrderManagementService.Domain.Entities.States.Exceptions.InvalidOrderStateException;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.UnitTests.Common.Builders;
using OrderManagementService.UnitTests.Common.Helpers;
using OrderManagementService.UnitTests.Common.Mothers;

namespace OrderManagementService.UnitTests.Domain.Entities
{
    public class OrderTests
    {
        [Fact]
        public void Create_Initializes_Order_In_Pending_State_With_Empty_Items_And_Zero_Total()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            var sut = Order.Create(customerId);

            // Assert
            sut.CustomerId.Should().Be(customerId);
            sut.State.Should().BeOfType<PendingState>();
            sut.Items.Should().BeEmpty();
            sut.TotalAmount.Value.Should().Be(0);
        }

        [Fact]
        public void AddOrderItem_Adds_Item_And_Updates_TotalAmount_When_In_Pending_State()
        {
            // Arrange
            var sut = OrderMother.EmptyPendingOrder();
            var productId = Guid.NewGuid();
            var price = Money.Create(150.00m);
            var quantity = Quantity.Create(2);

            // Act
            sut.AddOrderItem(productId, price, quantity);

            // Assert
            sut.Items.Should().HaveCount(1);
            sut.Items.First().ProductId.Should().Be(productId);
            sut.TotalAmount.Value.Should().Be(300.00m);
            sut.DomainEvents.Should().ContainSingle(e => e is OrderUpdatedEvent);
        }

        [Fact]
        public void AddOrderItem_Throws_DuplicateProductException_When_Product_Already_Exists_In_Order()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var sut = new OrderBuilder()
                .WithItem(productId, 100m, 1)
                .Build();

            // Act
            var act = () => sut.AddOrderItem(productId, Money.Create(100m), Quantity.Create(2));

            // Assert
            act.Should().Throw<DuplicateProductException>();
        }

        [Fact]
        public void AddOrderItem_Throws_InvalidOrderStateException_When_Not_In_Pending_State()
        {
            // Arrange
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.AddOrderItem(Guid.NewGuid(), Money.Create(50m), Quantity.Create(1));

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void RemoveOrderItem_Removes_Item_And_Recalculates_TotalAmount()
        {
            // Arrange
            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();
            var sut = new OrderBuilder()
                .WithItem(product1Id, 100m, 1)
                .WithItem(product2Id, 50m, 2)
                .Build();
            var itemToRemove = sut.Items.First(i => i.ProductId == product1Id);

            // Act
            sut.RemoveOrderItem(itemToRemove.Id);

            // Assert
            sut.Items.Should().HaveCount(1);
            sut.Items.Should().NotContain(i => i.Id == itemToRemove.Id);
            sut.TotalAmount.Value.Should().Be(100.00m);
        }

        [Fact]
        public void RemoveOrderItem_Throws_OrderItemNotFoundException_When_Item_Does_Not_Exist()
        {
            // Arrange
            var sut = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            var nonExistentItemId = Guid.NewGuid();

            // Act
            var act = () => sut.RemoveOrderItem(nonExistentItemId);

            // Assert
            act.Should().Throw<OrderItemNotFoundException>();
        }

        [Fact]
        public void RemoveOrderItem_Throws_InvalidOrderStateException_When_Not_In_Pending_State()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), productId, 100m, 1);
            var item = sut.Items.First();

            // Act
            var act = () => sut.RemoveOrderItem(item.Id);

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void Confirm_Transitions_To_Confirmed_State_And_Emits_OrderConfirmedEvent()
        {
            // Arrange
            var sut = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 200m, 1);
            sut.ClearEvents();

            // Act
            sut.Confirm();

            // Assert
            sut.State.Should().BeOfType<ConfirmedState>();
            sut.DomainEvents.Should().ContainSingle(e => e is OrderConfirmedEvent);
        }

        [Fact]
        public void Confirm_Throws_OrderMustHaveItemsException_When_Order_Has_No_Items()
        {
            // Arrange
            var sut = OrderMother.EmptyPendingOrder();

            // Act
            var act = () => sut.Confirm();

            // Assert
            act.Should().Throw<OrderMustHaveItemsException>();
        }

        [Fact]
        public void Confirm_Throws_InvalidOrderStateException_When_Order_Is_Already_Confirmed()
        {
            // Arrange
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.Confirm();

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void Ship_Transitions_To_Shipped_State_And_Emits_OrderShippedEvent_When_In_Confirmed_State()
        {
            // Arrange
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            sut.ClearEvents();

            // Act
            sut.Ship();

            // Assert
            sut.State.Should().BeOfType<ShippedState>();
            sut.DomainEvents.Should().ContainSingle(e => e is OrderShippedEvent);
        }

        [Fact]
        public void Ship_Throws_InvalidOrderStateException_When_Not_In_Confirmed_State()
        {
            // Arrange
            var sut = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.Ship();

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void Deliver_Transitions_To_Delivered_State_And_Emits_OrderDeliveredEvent_When_In_Shipped_State()
        {
            // Arrange
            var sut = OrderMother.ShippedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            sut.ClearEvents();

            // Act
            sut.Deliver();

            // Assert
            sut.State.Should().BeOfType<DeliveredState>();
            sut.DomainEvents.Should().ContainSingle(e => e is OrderDeliveredEvent);
        }

        [Fact]
        public void Deliver_Throws_InvalidOrderStateException_When_Not_In_Shipped_State()
        {
            // Arrange
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.Deliver();

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void Delete_Succeeds_And_Emits_OrderDeletedEvent_When_In_Pending_State()
        {
            // Arrange
            var sut = OrderMother.PendingOrderWithOneItem(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            sut.ClearEvents();

            // Act
            sut.Delete();

            // Assert
            var deletedEvent = sut.DomainEvents.OfType<OrderDeletedEvent>().SingleOrDefault();
            deletedEvent.Should().NotBeNull();
            deletedEvent!.State.Should().Be("Pending");
        }

        [Fact]
        public void Delete_Succeeds_And_Emits_OrderDeletedEvent_With_Previous_Confirmed_State_When_In_Confirmed_State()
        {
            // Arrange
            var sut = OrderMother.ConfirmedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);
            sut.ClearEvents();

            // Act
            sut.Delete();

            // Assert
            var deletedEvent = sut.DomainEvents.OfType<OrderDeletedEvent>().SingleOrDefault();
            deletedEvent.Should().NotBeNull();
            deletedEvent!.State.Should().Be("Confirmed");
        }

        [Fact]
        public void Delete_Throws_InvalidOrderStateException_When_In_Shipped_State()
        {
            // Arrange
            var sut = OrderMother.ShippedOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.Delete();

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }

        [Fact]
        public void Delete_Throws_InvalidOrderStateException_When_In_Delivered_State()
        {
            // Arrange
            var sut = OrderMother.DeliveredOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, 1);

            // Act
            var act = () => sut.Delete();

            // Assert
            act.Should().Throw<DomainInvalidOrderStateException>();
        }
    }
}
