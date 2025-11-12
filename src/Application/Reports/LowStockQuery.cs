using Application.Abstractions.Messaging;

namespace Application.Reports;

public record LowStockQuery(int LowStockThreshold) : IQuery<LowStockResponse>;