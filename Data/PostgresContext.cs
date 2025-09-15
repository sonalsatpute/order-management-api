using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Data;

// TECH DEBT: No interface, direct inheritance from DbContext
public class PostgresContext : DbContext
{
    // TECH DEBT: No constructor overloads for testing
    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
    }
    
    // TECH DEBT: Public DbSet without encapsulation
    public DbSet<Order> Orders { get; set; } = null!;
    
    // TECH DEBT: No configuration, relying on conventions
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TECH DEBT: Minimal configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.CustomerName).IsRequired();
            entity.Property(e => e.CustomerEmail).IsRequired();
            // TECH DEBT: Missing many configurations (indexes, constraints, etc.)
        });
        
        // TECH DEBT: OrderItem not configured properly
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasNoKey(); // TECH DEBT: Should have proper relationships
        });
        
        base.OnModelCreating(modelBuilder);
    }
    
    // TECH DEBT: No connection resilience
    // TECH DEBT: No query optimization
    // TECH DEBT: No audit trails
}