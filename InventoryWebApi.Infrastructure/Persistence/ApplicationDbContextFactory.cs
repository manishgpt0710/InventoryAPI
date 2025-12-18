using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InventoryWebApi.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances during migrations.
/// This is required when the DbContext is in a separate project from the startup project.
/// Reads connection string from environment variables or appsettings.json files.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Determine the base path for appsettings files
        // Try multiple possible locations to handle different execution contexts
        var basePath = GetBasePath();
        
        // Build configuration from appsettings files and environment variables
        // Priority: Environment variables > appsettings.Development.json > appsettings.json
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        var configuration = configurationBuilder.Build();
        
        // Get connection string from configuration
        // Check environment variable first, then configuration
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. " +
                "Please set it in one of the following:\n" +
                "1. Environment variable: ConnectionStrings__DefaultConnection\n" +
                "2. appsettings.json or appsettings.Development.json in the API project\n" +
                $"Current base path: {basePath}");
        }
        
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
    
    private static string GetBasePath()
    {
        // Get the directory where the Infrastructure project is located
        var infrastructurePath = Directory.GetCurrentDirectory();
        
        // If we're in the Infrastructure project directory, go up one level to solution root
        if (infrastructurePath.EndsWith("InventoryWebApi.Infrastructure", StringComparison.OrdinalIgnoreCase) ||
            infrastructurePath.Contains("InventoryWebApi.Infrastructure"))
        {
            var solutionRoot = Path.GetDirectoryName(infrastructurePath) 
                ?? Path.Combine(Directory.GetCurrentDirectory(), "..", "..");
            return Path.Combine(solutionRoot, "InventoryWebApi.Api");
        }
        
        // Try relative path from current directory
        var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "InventoryWebApi.Api");
        if (Directory.Exists(apiPath))
        {
            return apiPath;
        }
        
        // Try from solution root
        apiPath = Path.Combine(Directory.GetCurrentDirectory(), "InventoryWebApi.Api");
        if (Directory.Exists(apiPath))
        {
            return apiPath;
        }
        
        // Fallback to current directory (assumes running from API project)
        return Directory.GetCurrentDirectory();
    }
}

