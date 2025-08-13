using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StateManagement.Database.DataAccessLayer.Data;

/// <summary>
/// Helper to apply migrations for OrchestrationDbContext at application startup.
/// </summary>
public static class OrchestrationDbMigrator
{
    /// <summary>
    /// Applies any pending migrations for the OrchestrationDbContext.
    /// Call this at application startup after registering the DbContext.
    /// </summary>
    /// <param name="serviceProvider">The application's service provider.</param>
    public static void ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrchestrationDbContext>();
        dbContext.Database.Migrate();
    }
}
