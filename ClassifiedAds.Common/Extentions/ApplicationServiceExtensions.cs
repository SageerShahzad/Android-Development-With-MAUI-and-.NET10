using ClassifiedAds.Common.Data;
using ClassifiedAds.Common.Entities;
using ClassifiedAds.Common.Helpers;
using ClassifiedAds.Common.Interfaces;
using ClassifiedAds.Common.Repositories;
using ClassifiedAds.Common.Security;
using ClassifiedAds.Common.Services;
using ClassifiedAds.Common.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;


namespace ClassifiedAds.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Database
            services.AddDbContext<ClassifiedAdsDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            // CORS
            services.AddCors();

            // Custom Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IEncryptionService, E2eeEncryptionService>();
            services.AddScoped<IKeyManagementService, SimpleKeyManagementService>();
            services.AddScoped<LogUserActivity>();

            // Configuration
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // SignalR
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();

            // Controllers
            services.AddControllers();

            // Swagger (Development only)
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "ClassifiedAds API",
                        Version = "v1",
                        Description = "Classified Ads REST API"
                    });

                    // Configure file upload for IFormFile
                    c.MapType<IFormFile>(() => new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });
                });
            }

            return services;
        }

        public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ClassifiedAdsDbContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                await context.Database.MigrateAsync();
                await context.Connections.ExecuteDeleteAsync();
                await Seed.SeedUsers(userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger>();

                logger.LogError(ex, "An error occurred during migration");
                throw;
            }
        }
    }
}