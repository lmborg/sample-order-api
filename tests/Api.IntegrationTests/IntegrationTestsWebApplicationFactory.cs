using Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace Api.IntegrationTests;

public class IntegrationTestsWebApplicationFactory : WebApplicationFactory<Program>
{
    public FakeTimeProvider TimeProvider { get; } = new();
    public OrderApiDbContext? OrderApiDbContext { get; private set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OrderApiDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(TimeProvider);

            services.AddDbContext<OrderApiDbContext>(options =>
            {
                options.UseSqlite("Data Source=OrderApiTests.db");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            OrderApiDbContext = scopedServices.GetRequiredService<OrderApiDbContext>();
            OrderApiDbContext.Database.Migrate();
        });
    }
}