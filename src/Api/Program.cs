using Api.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

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