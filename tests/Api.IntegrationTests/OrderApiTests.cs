using System.Net;
using System.Text;
using System.Text.Json;

using Api.Requests;

using Application.Products.Commands;

using FluentAssertions;

namespace Api.IntegrationTests;

public class OrderApiTests(IntegrationTestsWebApplicationFactory application) : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    private readonly HttpClient _client = application.CreateClient();
    readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        var request = new CreateProductRequest("Test Product A", 115.75m, 12);
        
        var createProductResponse = await _client.PostAsync("products", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        createProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdProductContent = await createProductResponse.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<ProductResponse>(createdProductContent, _serializerOptions);
        createdProduct.Should().NotBeNull();
        createdProduct!.Id.Should().NotBeEmpty();
        createdProduct.Name.Should().Be("Test Product A");
        createdProduct.Price.Should().Be(115.75m);
        createdProduct.StockQuantity.Should().Be(12);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOk()
    {
        var productResponse = await _client.GetAsync("products");
        
        productResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetProductById_ReturnsNotFound_WithProblemDetails_WhenUnknownId()
    {
        var fakeProductId = Guid.NewGuid();
        var productResponse = await _client.GetAsync($"products/{fakeProductId}");
        
        productResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        productResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateProductPrice_ReturnsBadRequest_WhenPriceIsInvalid(decimal price)
    {
        var fakeProductId = Guid.NewGuid();
        var request = new UpdateProductPriceRequest(price);
        
        var response = await _client.PutAsync($"products/{fakeProductId}/price", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }
    
    [Fact]
    public async Task UpdateProductStock_ReturnsBadRequest_WhenQuantityIsInvalid()
    {
        var fakeProductId = Guid.NewGuid();
        var request = new UpdateProductStockRequest(-1);
        
        var response = await _client.PutAsync($"products/{fakeProductId}/stock", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }
    
    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WithProblemDetails_WhenUnknownId()
    {
        var fakeProductId = Guid.NewGuid();
        var productResponse = await _client.DeleteAsync($"products/{fakeProductId}");
        
        productResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        productResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
    }
    
    [Fact]
    public async Task DeleteProduct_SoftDeletes_WhichHidesItFromQueries()
    {
        var product = await CreateAProduct("Test Product A", 100m, 10);
        var deleteResponse = await _client.DeleteAsync($"products/{product.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"products/{product.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    private async Task<ProductResponse> CreateAProduct(string name, decimal price, int stockQuantity)
    {
        var request = new CreateProductRequest(name, price, stockQuantity);
        var createProductResponse = await _client.PostAsync("products", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        createProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdProductContent = await createProductResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProductResponse>(createdProductContent, _serializerOptions)!;
    }
}