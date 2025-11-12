using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Domain.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Commands;

public class DeleteProductCommandHandler(IOrderApiDbContext dbContext) : ICommandHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.SingleOrDefaultAsync(product => product.Id == command.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }
        
        dbContext.Products.Remove(product);
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}