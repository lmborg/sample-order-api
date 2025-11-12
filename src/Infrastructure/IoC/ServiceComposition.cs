using Application.Abstractions.Data;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.IoC;

public static class ServiceComposition
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderApiDbContext>(opt =>
        {
            var connectionString = configuration.GetConnectionString("OrderApi");
            opt.UseSqlite(connectionString);
        });
        
        services.AddScoped<IOrderApiDbContext>(sp => sp.GetRequiredService<OrderApiDbContext>());
        
        return services;
    }
}