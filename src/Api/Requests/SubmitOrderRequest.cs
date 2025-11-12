namespace Api.Requests;

public record SubmitOrderRequest(List<OrderItemRequest> OrderItems);