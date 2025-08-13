using Microsoft.EntityFrameworkCore;
using MamboBank.Gateway.WebApi.Models;

namespace MamboBank.Gateway.WebApi.Data;

public class MamboBankDbContext : DbContext
{
    public MamboBankDbContext(DbContextOptions<MamboBankDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd() // Configure auto-generated ID
                  .UseIdentityColumn(); // Use SQL Server Identity column
            entity.Property(e => e.UserId)
                  .IsRequired();
            entity.Property(e => e.Amount)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();
            entity.Property(e => e.TransactionType)
                  .IsRequired()
                  .HasMaxLength(20); // Increased to accommodate longer transaction types
            entity.Property(e => e.TransactionDate)
                  .HasDefaultValueSql("GETUTCDATE()")
                  .IsRequired();
            entity.Property(e => e.ExternalTransactionId)
                  .IsRequired()
                  .HasMaxLength(100); // Set maximum length for external transaction ID

            // Add indexes for better query performance
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionDate);
            
            // Add unique constraint on ExternalTransactionId
            entity.HasIndex(e => e.ExternalTransactionId).IsUnique();
        });
    }
}