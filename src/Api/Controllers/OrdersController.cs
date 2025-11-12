using Api.Requests;

using Application.Abstractions.Messaging;
using Application.Orders;
using Application.Orders.Commands;

using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(
    ICommandHandler<SubmitOrderCommand, OrderSummaryResponse> submitOrderCommandHandler) : ControllerBase
{
    [HttpPost]
    public async Task<OrderSummaryResponse> SubmitOrder(SubmitOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitOrderCommand(request.OrderItems.Select(oi => (oi.ProductId, oi.Quantity)).ToList());
        return await submitOrderCommandHandler.Handle(command, cancellationToken);
    }
}