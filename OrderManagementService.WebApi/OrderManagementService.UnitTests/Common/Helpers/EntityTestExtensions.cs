using System.Reflection;

namespace OrderManagementService.UnitTests.Common.Helpers
{
    public static class EntityTestExtensions
    {
        public static T WithId<T>(this T entity, Guid id) where T : class
        {
            var property = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, id);
            }
            else
            {
                var backingField = typeof(T).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                backingField?.SetValue(entity, id);
            }
            return entity;
        }

        public static void ClearEvents(this global::OrderManagementService.Domain.Entities.Base.Entity.AggregateRoot<Guid> aggregate)
        {
            aggregate.ClearDomainEvents();
        }
    }
}
