using Application.Abstractions.Messaging;

namespace Application.Products.Commands;

public record UpdateProductPriceCommand(Guid Id, decimal Price) : ICommand<ProductResponse>;