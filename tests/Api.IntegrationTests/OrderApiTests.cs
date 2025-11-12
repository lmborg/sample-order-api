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
}