using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Builders;

namespace OrderManagementService.UnitTests.Common.Mothers
{
    public static class CustomerMother
    {
        public static Customer JohnDoe()
            => new CustomerBuilder()
                .WithFullName("John Doe")
                .Build();

        public static Customer JaneSmith()
            => new CustomerBuilder()
                .WithFullName("Jane Smith")
                .Build();

        public static Customer WithSpecificId(Guid id, string fullName = "Standard Customer")
            => new CustomerBuilder()
                .WithId(id)
                .WithFullName(fullName)
                .Build();
    }
}
