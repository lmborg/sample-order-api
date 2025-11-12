using Api.Requests;
using Application.Abstractions.Messaging;
using Application.Products.Commands;
using Application.Products.Queries;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(
    ICommandHandler<CreateProductCommand, ProductResponse> createProductCommandHandler,
    IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResponse>> getAllProductsQueryHandler,
    IQueryHandler<GetProductByIdQuery, ProductResponse> getProductByIdQueryHandler,
    ICommandHandler<UpdateProductStockCommand, ProductResponse> updateProductStockCommandHandler,
    ICommandHandler<UpdateProductPriceCommand, ProductResponse> updateProductPriceCommandHandler) : ControllerBase
{
    [HttpPost]
    public async Task<ProductResponse> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Price, request.StockQuantity);
        return await createProductCommandHandler.Handle(command, cancellationToken);
    }
    
    [HttpGet]
    public async Task<IEnumerable<ProductResponse>> GetAllProducts(CancellationToken cancellationToken)
    {
        return await getAllProductsQueryHandler.Handle(new GetAllProductsQuery(), cancellationToken);
    }
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<ProductResponse> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        return await getProductByIdQueryHandler.Handle(new GetProductByIdQuery(id), cancellationToken);
    }
    
    [HttpPut]
    [Route("{productId:guid}/stock")]
    public async Task<ProductResponse> UpdateProductStock([FromRoute] Guid productId, UpdateProductStockRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductStockCommand(productId, request.StockQuantity);
        return await updateProductStockCommandHandler.Handle(command, cancellationToken);
    }
    
    [HttpPut]
    [Route("{productId:guid}/price")]
    public async Task<ProductResponse> UpdateProductPrice([FromRoute] Guid productId, UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductPriceCommand(productId, request.Price);
        return await updateProductPriceCommandHandler.Handle(command, cancellationToken);
    }
}