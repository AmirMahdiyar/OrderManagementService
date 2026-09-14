using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagementService.Activators.Middlewares;
using OrderManagementService.Application.Contracts;
using OrderManagementService.Domain.Services;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Data.Interceptors;
using OrderManagementService.Infrastructure.Helper;
using OrderManagementService.Infrastructure.Security;
using OrderManagementService.Infrastructure.Security.Options;
using Serilog;
using System.Text;

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
        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<InsertOutboxMessagesInterceptor>();

            builder.Services.AddDbContext<OrderManagementDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();

                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(interceptor);
            });

            return builder;
        }
        public static WebApplicationBuilder AddSingletonDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPasswordHasher, Md5PasswordHasher>();
            return builder;
        }
        public static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddScoped<IJwtProvider, JwtProvider>();

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            builder.Services.AddAuthorization();

            return builder;
        }
    }
}
