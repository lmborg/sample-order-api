namespace Domain.Entities;

public class OrderItem
{
    public required Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}