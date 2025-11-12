namespace Application.Products.Commands;

public record ProductResponse(Guid Id, string Name, decimal Price, int StockQuantity);