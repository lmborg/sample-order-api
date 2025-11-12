using Api.ExceptionHandlers;

namespace Api.IoC;

public static class ServiceComposition
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<DefaultExceptionHandler>();
        services.AddProblemDetails();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();
        
        return services;
    }
}