using Microsoft.EntityFrameworkCore;
using OrderManagementService.Domain.Services;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Data.Seeding;

namespace OrderManagementService.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<OrderManagementDbContext>();
                var passwordHasher = services.GetRequiredService<IPasswordHasher>();

                await context.Database.MigrateAsync();

                await DatabaseSeeder.SeedAsync(context, passwordHasher);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }
        }
    }
}
