using Application.Abstractions.Messaging;

namespace Application.Reports;

public record DailySummaryQuery(DateOnly ReportDate) : IQuery<DailySummaryResponse>;