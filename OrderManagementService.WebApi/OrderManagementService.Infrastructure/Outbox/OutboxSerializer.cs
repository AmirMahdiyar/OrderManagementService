using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace OrderManagementService.Infrastructure.Outbox
{
    public class OutboxJsonContractResolver : DefaultContractResolver
    {
        public static readonly OutboxJsonContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable && member is PropertyInfo pi)
            {
                var hasPrivateSetter = pi.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;
            }

            return prop;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);

            if (objectType.IsValueType && !objectType.IsPrimitive && !objectType.IsEnum)
            {
                var ctor = objectType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(c => c.GetParameters().Length == 1);

                if (ctor != null)
                {
                    contract.OverrideCreator = args => ctor.Invoke(args);
                    contract.CreatorParameters.Clear();
                    var p = ctor.GetParameters()[0];
                    var prop = contract.Properties.FirstOrDefault(x => string.Equals(x.PropertyName, p.Name, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        contract.CreatorParameters.Add(prop);
                    }
                }
            }

            return contract;
        }
    }

    public static class OutboxSerializer
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = OutboxJsonContractResolver.Instance,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };
    }
}
