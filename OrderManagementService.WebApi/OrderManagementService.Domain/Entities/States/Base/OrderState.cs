using OrderManagementService.Domain.Entities.States.Exceptions;

namespace OrderManagementService.Domain.Entities.States.Base
{
    public abstract class OrderState
    {
        public abstract string Name { get; }

        public virtual void Confirm(Order order) =>
            throw new InvalidOrderStateException($"Cannot confirm order from {Name} state.");

        public virtual void Ship(Order order) =>
            throw new InvalidOrderStateException($"Cannot ship order from {Name} state.");

        public virtual void Deliver(Order order) =>
            throw new InvalidOrderStateException($"Cannot deliver order from {Name} state.");
    }
}
