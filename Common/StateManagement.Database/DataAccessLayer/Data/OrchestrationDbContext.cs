using Microsoft.EntityFrameworkCore;
using StateManagement.Database.DataAccessLayer.Contracts;

namespace StateManagement.Database.DataAccessLayer.Data;

public class OrchestrationDbContext : DbContext
{
    public OrchestrationDbContext(DbContextOptions<OrchestrationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProcessState> ProcessStates { get; set; } = null!;

    public DbSet<ProcessRequest> ProcessStateRequests { get; set; } = null!;

    public DbSet<ProcessResponse> ProcessResponses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureProcessStates(modelBuilder);
        ConfigureProcessStateRequests(modelBuilder);
    }

    private static void ConfigureProcessStates(ModelBuilder modelBuilder)
    {
        var processStateEntity = modelBuilder.Entity<ProcessState>();

        // Configure the foreign key relationship
        processStateEntity
            .HasOne(ps => ps.CreateRequest)
            .WithMany(psr => psr.ProcessStates)
            .HasForeignKey(ps => ps.CreateRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        processStateEntity
            .HasIndex(e => new { e.CreateRequestId })
            .HasDatabaseName("IX_ProcessStates__CreateRequestId_BrandId");

        processStateEntity
            .HasIndex(e => e.LastModifiedDateTime)
            .HasDatabaseName("IX_ProcessStates__LastModifiedDateTime");
    }

    private static void ConfigureProcessStateRequests(ModelBuilder modelBuilder)
    {
        var processStateEntity = modelBuilder.Entity<ProcessRequest>();

        processStateEntity
            .HasIndex(e => e.RequestDateTime)
            .HasDatabaseName("IX_ProcessStateRequests__RequestDateTime");

        processStateEntity
            .HasIndex(e => e.ExternalRequestId)
            .IsUnique()
            .HasDatabaseName("IX_ProcessStateRequests__ExternalRequestId");
    }
}
