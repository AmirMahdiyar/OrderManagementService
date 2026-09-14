namespace OrderManagementService.Domain.Entities.Base.Entity
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id is null || Id.Equals(default) || other.Id.Equals(default)) return false;

            return Id.Equals(other.Id);
        }

        protected abstract TId InitialId();

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }

        public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return !(left == right);
        }
    }
}
