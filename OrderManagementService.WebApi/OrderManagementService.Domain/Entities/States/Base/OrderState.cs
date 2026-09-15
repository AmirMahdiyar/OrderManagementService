using OrderManagementService.Domain.Entities.States.Exceptions;

namespace OrderManagementService.Domain.Entities.States.Base
{
    public abstract class OrderState : IEquatable<OrderState>
    {
        public static readonly string[] ValidStates = ["Pending", "Confirmed", "Shipped", "Delivered"];

        public static bool IsValid(string? stateName) =>
            !string.IsNullOrWhiteSpace(stateName) &&
            ValidStates.Any(s => s.Equals(stateName.Trim(), StringComparison.OrdinalIgnoreCase));

        public static string? Normalize(string? stateName) =>
            ValidStates.FirstOrDefault(s => s.Equals(stateName?.Trim(), StringComparison.OrdinalIgnoreCase));

        public abstract string Name { get; }

        public bool Equals(OrderState? other) => other is not null && Name == other.Name;

        public override bool Equals(object? obj) => obj is OrderState other && Equals(other);

        public override int GetHashCode() => Name.GetHashCode();

        public static bool operator ==(OrderState? left, OrderState? right) => Equals(left, right);

        public static bool operator !=(OrderState? left, OrderState? right) => !Equals(left, right);

        public virtual void Confirm(Order order) =>
            throw new InvalidOrderStateException($"Cannot confirm order from {Name} state.");

        public virtual void Ship(Order order) =>
            throw new InvalidOrderStateException($"Cannot ship order from {Name} state.");

        public virtual void Deliver(Order order) =>
            throw new InvalidOrderStateException($"Cannot deliver order from {Name} state.");
    }
}
