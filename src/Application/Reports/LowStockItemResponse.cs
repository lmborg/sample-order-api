namespace Application.Reports;

public record LowStockItemResponse(Guid ProductId, int StockQuantity);