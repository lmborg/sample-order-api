using Application.Abstractions.Messaging;

namespace Application.Products.Commands;

public record UpdateProductStockCommand(Guid Id, int StockQuantity) : ICommand<ProductResponse>;