using Application.Abstractions.Messaging;
using Application.Products.Commands;

namespace Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;