using Application.Abstractions.Messaging;

namespace Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : ICommand;