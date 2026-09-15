using OrderManagementService.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSerilogLogging()
    .AddDatabase()
    .AddSingletonDependencies()
    .AddDomainServices()
    .AddRepositories()
    .AddApplicationServices()
    .AddBackgroundJobs()
    .AddJwtAuthentication()
    .AddSwaggerDocumentation()
    .AddWebServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management Service API v1");
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Applying migrations and seeding database...");

    await app.ApplyMigrationsAndSeedAsync();

    Log.Information("Starting Order Management Service...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}