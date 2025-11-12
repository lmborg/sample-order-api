using Api.IoC;
using Application.IoC;

using Infrastructure.Data;
using Infrastructure.IoC;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddApiServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderApiDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.UseExceptionHandler();

app.Run();

namespace Api // for integration tests
{
    public partial class Program;
}