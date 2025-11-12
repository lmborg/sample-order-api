namespace Domain.Entities;

public class Order
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public decimal TotalAmount { get; init; }
    public List<OrderItem> Items { get; init; } = [];
}