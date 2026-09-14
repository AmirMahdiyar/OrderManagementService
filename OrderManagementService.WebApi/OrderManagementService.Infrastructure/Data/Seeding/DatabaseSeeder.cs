using Bogus;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Domain.Entities.ValueObjects;
using OrderManagementService.Domain.Services;

namespace OrderManagementService.Infrastructure.Data.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(OrderManagementDbContext context, IPasswordHasher passwordHasher)
        {
            if (await context.Users.AnyAsync())
                return;

            var adminUser = User.Create("admin", "Admin@123", "Admin", passwordHasher);
            var testUser = User.Create("testuser", "User@123", "User", passwordHasher);

            var usersToInsert = new List<User> { adminUser, testUser };

            var userFaker = new Faker<User>()
                .CustomInstantiator(f => User.Create(
                    username: f.Internet.UserName(),
                    plainPassword: "Password123",
                    role: "User",
                    passwordHasher: passwordHasher));

            usersToInsert.AddRange(userFaker.Generate(48));

            await context.Users.AddRangeAsync(usersToInsert);
            await context.SaveChangesAsync();

            var customerFaker = new Faker<Customer>()
                .CustomInstantiator(f => Customer.Create(
                    fullName: f.Name.FullName(),
                    userId: usersToInsert[f.IndexFaker].Id));

            var customers = customerFaker.Generate(50);
            await context.Customers.AddRangeAsync(customers);

            var productFaker = new Faker<Product>()
                .CustomInstantiator(f => Product.Create(
                    name: f.Commerce.ProductName(),
                    stock: Quantity.Create(f.Random.Int(10, 500)),
                    price: Money.Create(Math.Round(f.Random.Decimal(10, 1000), 2))));

            var products = productFaker.Generate(200);
            await context.Products.AddRangeAsync(products);

            await context.SaveChangesAsync();
        }
    }
}
