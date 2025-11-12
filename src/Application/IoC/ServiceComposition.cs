using Application.Abstractions.Messaging;
using Application.Products.Commands;

using Microsoft.Extensions.DependencyInjection;

namespace Application.IoC;

public static class ServiceComposition
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
        
        return services;
    }

}