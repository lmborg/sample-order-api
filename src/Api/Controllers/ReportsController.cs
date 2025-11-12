using Application.Abstractions.Messaging;
using Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController(
    IQueryHandler<DailySummaryQuery, DailySummaryResponse> dailySummaryQueryHandler,
    IQueryHandler<LowStockQuery, LowStockResponse> lowStockQueryHandler) : ControllerBase
{
    [HttpGet]
    [Route("daily-summary")]
    public async Task<DailySummaryResponse> DailySummary(CancellationToken cancellationToken)
    {
        var command = new DailySummaryQuery(DateOnly.FromDateTime(DateTime.Now)); // todo: allow as query param
        return await dailySummaryQueryHandler.Handle(command, cancellationToken);
    }
    
    [HttpGet]
    [Route("low-stock")]
    public async Task<LowStockResponse> LowStock([FromQuery] int threshold = 5, CancellationToken cancellationToken = default)
    {
        var command = new LowStockQuery(threshold);
        return await lowStockQueryHandler.Handle(command, cancellationToken);
    }
}