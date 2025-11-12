using Application.Abstractions.Messaging;
using Application.Products.Commands;

namespace Application.Products.Queries;

public record GetAllProductsQuery : IQuery<IEnumerable<ProductResponse>>;