using Api.Requests;
using Application.Abstractions.Messaging;
using Application.Products.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(
    ICommandHandler<CreateProductCommand, ProductResponse> createProductCommandHandler) : ControllerBase
{
    [HttpPost]
    public async Task<ProductResponse> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Price, request.StockQuantity);
        return await createProductCommandHandler.Handle(command, cancellationToken);
    }
}