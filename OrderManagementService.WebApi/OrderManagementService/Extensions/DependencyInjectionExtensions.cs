using OrderManagementService.Activators.Middlewares;
using Serilog;

namespace OrderManagementService.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebApplicationBuilder AddWebServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            return builder;
        }
        public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }
    }
}
