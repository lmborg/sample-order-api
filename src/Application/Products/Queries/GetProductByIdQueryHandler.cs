using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products.Commands;

using Domain.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries;

public class GetProductByIdQueryHandler(IOrderApiDbContext dbContext) : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == query.Id, cancellationToken);
        
        return product is null ? 
            throw new ProductNotFoundException(query.Id) : 
            new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity);
    }
}