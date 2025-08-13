using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StateManagement.Database.DataAccessLayer.Contracts;
using StateManagement.Database.DataAccessLayer.Data;
using StateManagement.Database.Models;
using StateManagement.Database.Services;
using System.Text.Json;

namespace StateManagement.Database.Configuration;

public static class ServiceCollectionForDataAccessLayer
{
    /// <summary>
    /// Add SessionState Data Access Layer services with DbContext options configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOrchestration(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<OrchestrationDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        // Register the ProcessState service
        services.AddScoped<ProcessService>();

        return services;
    }
}
