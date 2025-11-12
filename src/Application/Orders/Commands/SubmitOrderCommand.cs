using Application.Abstractions.Messaging;

namespace Application.Orders.Commands;

public record SubmitOrderCommand(List<(Guid ProductId, int Quantity)> OrderItems) : ICommand<OrderSummaryResponse>;