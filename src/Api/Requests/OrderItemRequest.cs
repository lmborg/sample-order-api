namespace Api.Requests;

public record OrderItemRequest(Guid ProductId, int Quantity);