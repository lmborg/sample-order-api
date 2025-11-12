using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Domain.Entities;

using FluentValidation;

namespace Application.Products.Commands;

public class CreateProductCommandHandler(IOrderApiDbContext dbContext, IValidator<CreateProductCommand> validator) : ICommandHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        validator.ValidateAndThrow(command);
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Price = command.Price,
            StockQuantity = command.StockQuantity
        };
        
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}