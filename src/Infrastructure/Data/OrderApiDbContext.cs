using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class OrderApiDbContext(DbContextOptions<OrderApiDbContext> options) : DbContext(options), IOrderApiDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(200);
            b.Property(p => p.Price).IsRequired();
            b.Property(p => p.StockQuantity).IsRequired();
            b.Property(p => p.IsDeleted).HasDefaultValue(false);
            b.Property(p => p.Version).IsConcurrencyToken();
            
            b.HasQueryFilter(p => !p.IsDeleted);
        });
        
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.CreatedAtUtc).IsRequired();
            b.Property(o => o.TotalAmount).IsRequired();
            b.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
        });

        modelBuilder.Entity<OrderItem>(b =>
        {
            b.HasKey(oi => oi.Id);
            b.Property(oi => oi.UnitPrice).IsRequired();
            b.Property(oi => oi.Quantity).IsRequired();
            b.Property(oi => oi.ProductId).IsRequired();
        });
    }
}