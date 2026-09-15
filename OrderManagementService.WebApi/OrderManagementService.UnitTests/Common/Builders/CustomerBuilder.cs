using OrderManagementService.Domain.Entities;
using OrderManagementService.UnitTests.Common.Helpers;

namespace OrderManagementService.UnitTests.Common.Builders
{
    public class CustomerBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _fullName = "John Doe";
        private Guid _userId = Guid.NewGuid();

        public CustomerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CustomerBuilder WithFullName(string fullName)
        {
            _fullName = fullName;
            return this;
        }

        public CustomerBuilder WithUserId(Guid userId)
        {
            _userId = userId;
            return this;
        }

        public Customer Build()
        {
            var customer = Customer.Create(_fullName, _userId);
            customer.WithId(_id);
            return customer;
        }
    }
}
