namespace Api.Requests;

public record CreateProductRequest(string Name, decimal Price, int StockQuantity);