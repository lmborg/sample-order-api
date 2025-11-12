using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Products.Commands;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries;

public class GetAllProductsQueryHandler(IOrderApiDbContext dbContext) : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .Select(product => new ProductResponse(product.Id, product.Name, product.Price, product.StockQuantity))
            .ToListAsync(cancellationToken);
    }
}