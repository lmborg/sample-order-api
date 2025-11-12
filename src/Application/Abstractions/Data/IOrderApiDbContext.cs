using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IOrderApiDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}