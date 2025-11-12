using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Microsoft.EntityFrameworkCore;

namespace Application.Reports;

public class LowStockQueryHandler(IOrderApiDbContext dbContext) : IQueryHandler<LowStockQuery, LowStockResponse>
{
    public async Task<LowStockResponse> Handle(LowStockQuery query, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products.AsNoTracking()
            .Where(p => p.StockQuantity < query.LowStockThreshold)
            .ToListAsync(cancellationToken);

        return new LowStockResponse(products.Select(p => new LowStockItemResponse(p.Id, p.StockQuantity)).ToList());
    }
}