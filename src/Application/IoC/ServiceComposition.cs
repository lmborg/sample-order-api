using Application.Abstractions.Messaging;
using Application.Products.Commands;
using Application.Products.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Application.IoC;

public static class ServiceComposition
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
        services.AddTransient<IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>, GetAllProductsQueryHandler>();
        services.AddTransient<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();

        return services;
    }

}