using System.Text;
using EngagementTracker.Core.Interfaces;
using EngagementTracker.Core.Services;
using EngagementTracker.Infrastructure.Data;
using EngagementTracker.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EngagementTracker.Api.Extensions;

/// <summary>
/// Extension methods for registering application services in the DI container.
/// Keeps Program.cs clean by grouping related registrations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the MySQL database context using the connection string from configuration.
    /// </summary>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<DbSeeder>();

        return services;
    }

    /// <summary>
    /// Registers all application services and repositories.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEngagementRepository, EngagementRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEngagementService, EngagementService>();
        services.AddScoped<ITimeEntryService, TimeEntryService>();

        return services;
    }

    /// <summary>
    /// Registers JWT authentication using the secret and settings from configuration.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        string secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");

        services.AddAuthentication(options =>
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
                ValidIssuer = configuration["Jwt:Issuer"] ?? "EngagementTracker",
                ValidAudience = configuration["Jwt:Audience"] ?? "EngagementTracker",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Registers FluentValidation validators from the Core assembly.
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Core.Validators.CreateEngagementValidator>();

        return services;
    }

    /// <summary>
    /// Configures Swagger/OpenAPI with JWT bearer authentication support.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Engagement Tracker API",
                Version = "v1",
                Description = "RESTful API for managing client engagements, time tracking, and budget monitoring at BDO."
            });

            // JWT Bearer auth in Swagger UI
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: eyJhbGciOiJ..."
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

            // Include XML comments from all projects
            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string xmlFile in xmlFiles)
            {
                options.IncludeXmlComments(xmlFile);
            }
        });

        return services;
    }
}
