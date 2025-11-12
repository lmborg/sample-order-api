using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Domain.Exceptions;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands;

public class UpdateProductPriceCommandHandler(IOrderApiDbContext dbContext, IValidator<UpdateProductPriceCommand> validator) : ICommandHandler<UpdateProductPriceCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductPriceCommand command, CancellationToken cancellationToken)
    {
        validator.ValidateAndThrow(command);
        
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == command.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }
        
        product.Price = command.Price;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}