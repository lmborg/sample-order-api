namespace Domain.Entities;

public class Product
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsDeleted { get; set; }
}