using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Microsoft.EntityFrameworkCore;

namespace Application.Reports;

public class DailySummaryQueryHandler(IOrderApiDbContext dbContext) : IQueryHandler<DailySummaryQuery, DailySummaryResponse>
{
    public async Task<DailySummaryResponse> Handle(DailySummaryQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders.AsNoTracking()
            .Where(o => DateOnly.FromDateTime(o.CreatedAtUtc) == query.ReportDate)
            .ToListAsync(cancellationToken);

        return new DailySummaryResponse(orders.Count, orders.Sum(o => o.TotalAmount));
    }
}