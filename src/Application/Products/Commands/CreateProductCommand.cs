using Application.Abstractions.Messaging;

namespace Application.Products.Commands;

public record CreateProductCommand(string Name, decimal Price, int StockQuantity) : ICommand<ProductResponse>;