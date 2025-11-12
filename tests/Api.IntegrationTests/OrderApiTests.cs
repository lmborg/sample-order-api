using System.Net;
using System.Text;
using System.Text.Json;
using Api.Requests;
using Application.Orders;
using Application.Products.Commands;
using FluentAssertions;

using Infrastructure.Data;

using Microsoft.Extensions.Time.Testing;

namespace Api.IntegrationTests;

public class OrderApiTests(IntegrationTestsWebApplicationFactory application) : IClassFixture<IntegrationTestsWebApplicationFactory>
{
    private readonly HttpClient _client = application.CreateClient();
    private readonly FakeTimeProvider _timeProvider = application.TimeProvider;
    private readonly OrderApiDbContext _dbContext = application.OrderApiDbContext!;
    readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

    [Fact]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        var request = new CreateProductRequest("Test Product A", 115.75m, 12);
        
        var productResponse = await _client.PostAsync("products", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        productResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var productResponseContent = await productResponse.Content.ReadAsStringAsync();
        var createdProduct = JsonSerializer.Deserialize<ProductResponse>(productResponseContent, _serializerOptions);
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
    
    [Fact]
    public async Task SubmitOrder_ReducesProductStock_ReturnsOrderSummary()
    {
        _timeProvider.SetUtcNow(new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc));
        var product1 = await CreateAProduct("Order Product 1", 100m, 10);
        var product2 = await CreateAProduct("Order Product 2", 55.50m, 10);
        var request = new SubmitOrderRequest([ new OrderItemRequest(product1.Id, 2), new OrderItemRequest(product2.Id, 1) ]);
        
        var submitOrderResponse = await _client.PostAsync("orders", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        submitOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderSummaryContent = await submitOrderResponse.Content.ReadAsStringAsync();
        var orderSummary = JsonSerializer.Deserialize<OrderSummaryResponse>(orderSummaryContent, _serializerOptions);
        orderSummary.Should().NotBeNull();
        orderSummary!.Id.Should().NotBeEmpty();
        orderSummary.TotalOrderAmount.Should().Be(255.50m);
        orderSummary.OrderItems.Count.Should().Be(2);
        orderSummary.OrderTimestamp.Should().Be(_timeProvider.GetUtcNow().DateTime.ToLocalTime());
        
        var product = await GetProduct(product1);
        product!.StockQuantity.Should().Be(8);
        
        product = await GetProduct(product2);
        product!.StockQuantity.Should().Be(9);
    }

    [Fact]
    public async Task SubmitOrder_ReturnsError_WhenInsufficientStock()
    {
        var product = await CreateAProduct("Order Product X", 100m, 1);
        var request = new SubmitOrderRequest([ new OrderItemRequest(product.Id, 2) ]);
        
        var submitOrderResponse = await _client.PostAsync("orders", new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
        
        submitOrderResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void UpdatingProductStock_Fails_WhenVersionIsNotCurrent()
    {
        // todo: implement this test to ensure that the product version check prevents race conditions        
    }
    
    private async Task<ProductResponse?> GetProduct(ProductResponse product1)
    {
        var productResponse = await _client.GetAsync($"products/{product1.Id}");
        productResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var productResponseContent = await productResponse.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductResponse>(productResponseContent, _serializerOptions);
        return product;
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