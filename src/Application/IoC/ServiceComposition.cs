using Application.Abstractions.Messaging;
using Application.Orders;
using Application.Orders.Commands;
using Application.Products.Commands;
using Application.Products.Queries;
using Application.Reports;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Application.IoC;

public static class ServiceComposition
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<CreateProductCommand, ProductResponse>, CreateProductCommandHandler>();
        services.AddTransient<IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>, GetAllProductsQueryHandler>();
        services.AddTransient<IQueryHandler<GetProductByIdQuery, ProductResponse>, GetProductByIdQueryHandler>();
        services.AddTransient<ICommandHandler<UpdateProductPriceCommand, ProductResponse>, UpdateProductPriceCommandHandler>();
        services.AddTransient<ICommandHandler<UpdateProductStockCommand, ProductResponse>, UpdateProductStockCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteProductCommand>, DeleteProductCommandHandler>();
        
        services.AddTransient<ICommandHandler<SubmitOrderCommand, OrderSummaryResponse>, SubmitOrderCommandHandler>();

        services.AddTransient<IQueryHandler<DailySummaryQuery, DailySummaryResponse>, DailySummaryQueryHandler>();

        services.AddSingleton(TimeProvider.System);

        services.AddValidatorsFromAssembly(typeof(ServiceComposition).Assembly);

        return services;
    }

}