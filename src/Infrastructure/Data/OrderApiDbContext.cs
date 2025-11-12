using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class OrderApiDbContext(DbContextOptions<OrderApiDbContext> options) : DbContext(options), IOrderApiDbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(200);
            b.Property(p => p.Price).IsRequired();
            b.Property(p => p.StockQuantity).IsRequired();
            b.Property(p => p.IsDeleted).HasDefaultValue(false);
            
            b.HasQueryFilter(p => !p.IsDeleted);
        });
    }
}