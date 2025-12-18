using ClassifiedAds.Common.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ClassifiedAds.API.Extensions
{
    public static class WebAppExtensions
    {
        public static WebApplication UseApplicationInitialization(this WebApplication app)
        {
            // 1. Global exception handler
            app.UseExceptionHandler(errApp =>
            {
                errApp.Run(async context =>
                {
                    var ex = context.Features
                                    .Get<IExceptionHandlerFeature>()
                                    ?.Error;

                    context.Response.ContentType = "application/json";

                    if (ex is NpgsqlException)
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Database unavailable",
                            message = "Please try again later.",
                            code = "DB_CONNECTION_FAILED"
                        });
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Internal server error",
                            message = "An unexpected error occurred."
                        });
                    }
                });
            });

            // 2. Database migration (and optional seeding)
            using (var scope = app.Services.CreateScope())
            {
                var svcProvider = scope.ServiceProvider;
                var ctx = svcProvider.GetRequiredService<ClassifiedAdsDbContext>();

                // <— Resolve ILoggerFactory instead of ILogger<T>
                var loggerFactory = svcProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("AppInitialization");

                var cfg = svcProvider.GetRequiredService<IConfiguration>();
                var connString = cfg.GetConnectionString("DefaultConnection");
                var builder = new NpgsqlConnectionStringBuilder(connString);

                try
                {
                    ctx.Database.Migrate();
                    logger.LogInformation("✅ Database migrated successfully.");
                }
                catch (NpgsqlException)
                {
                    logger.LogError(
                        "❌ Could not connect to the database at Database Server. " +
                        "Please ensure Databse Server is running and the connection string is correct.");
                        //builder.Host, builder.Port);
                    // 2) Terminate the process so we don’t fall into an unhandled exception
                    Environment.Exit(1);
                   // throw; // fail‑fast
                }
            }

            return app;
        }
    }
}
