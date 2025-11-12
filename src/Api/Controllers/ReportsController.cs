using Application.Abstractions.Messaging;
using Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController(IQueryHandler<DailySummaryQuery, DailySummaryResponse> dailySummaryQueryHandler) : ControllerBase
{
    [HttpGet]
    public async Task<DailySummaryResponse> DailySummary(CancellationToken cancellationToken)
    {
        var command = new DailySummaryQuery(DateOnly.FromDateTime(DateTime.Now));
        return await dailySummaryQueryHandler.Handle(command, cancellationToken);
    }
}