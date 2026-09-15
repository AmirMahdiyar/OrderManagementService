using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagementService.Activators.Jobs;
using OrderManagementService.Activators.Middlewares;
using OrderManagementService.Application.Contracts.Jwt;
using OrderManagementService.Application.Contracts.Repositories;
using OrderManagementService.Application.Contracts.UnitOfWork;
using OrderManagementService.Application.PipelineBehaviors;
using OrderManagementService.Domain.Services;
using OrderManagementService.Domain.Services.DomainServices.OrderConfirmation;
using OrderManagementService.Domain.Services.DomainServices.UserPassword;
using OrderManagementService.Infrastructure;
using OrderManagementService.Infrastructure.Data.Interceptors;
using OrderManagementService.Infrastructure.Repositories;
using OrderManagementService.Infrastructure.Security;
using OrderManagementService.Infrastructure.Security.Options;
using OrderManagementService.Infrastructure.Services;
using Serilog;
using System.Text;

namespace OrderManagementService.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static WebApplicationBuilder AddSwaggerDocumentation(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Order Management Service API",
                    Version = "v1",
                    Description = "RESTful API for Order Management Service"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token to authenticate."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return builder;
        }
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
            builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
            return builder;
        }

        public static WebApplicationBuilder AddDomainServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IOrderInventoryChecker, OrderInventoryChecker>();
            builder.Services.AddScoped<IOrderConfirmationDomainService, OrderConfirmationDomainService>();
            builder.Services.AddScoped<IUserPasswordDomainService, UserPasswordDomainService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            return builder;
        }

        public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICustomerCommandRepository, CustomerCommandRepository>();
            builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
            builder.Services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
            builder.Services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
            builder.Services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
            builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
            return builder;
        }

        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            var applicationAssembly = typeof(IUnitOfWork).Assembly;

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssembly(applicationAssembly);

            return builder;
        }

        public static WebApplicationBuilder AddBackgroundJobs(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<OutboxBackgroundService>();
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

