namespace Application.Orders;

public record OrderSummaryResponse(Guid Id, DateTimeOffset OrderTimestamp, decimal TotalOrderAmount, List<OrderItemResponse> OrderItems);