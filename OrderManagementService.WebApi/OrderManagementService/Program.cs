using OrderManagementService.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSerilogLogging()
    .AddWebServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
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