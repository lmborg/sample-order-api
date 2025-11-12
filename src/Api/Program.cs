using Api.IoC;
using Application.IoC;
using Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddApiServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

namespace Api
{
    public partial class Program;
}