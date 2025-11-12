using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Domain.Exceptions;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands;

public class UpdateProductStockCommandHandler(IOrderApiDbContext dbContext, IValidator<UpdateProductStockCommand> validator) : ICommandHandler<UpdateProductStockCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductStockCommand command, CancellationToken cancellationToken)
    {
        validator.ValidateAndThrow(command);
        
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == command.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }
        
        product.StockQuantity = command.StockQuantity;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}