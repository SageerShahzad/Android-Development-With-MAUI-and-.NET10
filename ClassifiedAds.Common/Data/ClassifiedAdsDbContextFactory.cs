// ClassifiedAds.Migrations/Program.cs
using System.Reflection;
using ClassifiedAds.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

// This factory is used by EF Core tools
public class ClassifiedAdsDbContextFactory : IDesignTimeDbContextFactory<ClassifiedAdsDbContext>
{
    public ClassifiedAdsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ClassifiedAdsDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new ClassifiedAdsDbContext(optionsBuilder.Options);
    }
}

// ClassifiedAds.Migrations/MigrationRunner.cs
public class MigrationRunner
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Running migrations...");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ClassifiedAdsDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        using var context = new ClassifiedAdsDbContext(optionsBuilder.Options);
        context.Database.Migrate();

        Console.WriteLine("Migrations completed!");
    }
}