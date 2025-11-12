using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Domain.Entities;
using Domain.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands;

public class SubmitOrderCommandHandler(IOrderApiDbContext dbContext, TimeProvider timeProvider) : ICommandHandler<SubmitOrderCommand, OrderSummaryResponse>
{
    public async Task<OrderSummaryResponse> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        await using var tx = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var ids = command.OrderItems.Select(i => i.ProductId).Distinct().ToList();
        var products = await dbContext.Products.Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            product.StockQuantity -= command.OrderItems.Where(oi => oi.ProductId == product.Id).Sum(oi => oi.Quantity);
        }

        if (products.Any(p => p.StockQuantity < 0))
        {
            throw new InsufficientProductStockException(products.Where(p => p.StockQuantity < 0).Select(p => p.Id)
                .ToList());
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);

        var orderId = Guid.NewGuid();
        var productsById = products.ToDictionary(p => p.Id);
        var orderItems = command.OrderItems.Select(oi =>
                new OrderItem { Id = Guid.NewGuid(), ProductId = oi.ProductId, Quantity = oi.Quantity, UnitPrice = productsById[oi.ProductId].Price})
            .ToList();
        
        var order = new Order
        {
            Id = orderId,
            CreatedAtUtc = timeProvider.GetUtcNow().DateTime,
            TotalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
            Items = orderItems
        };
        
        await dbContext.Orders.AddAsync(order, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return new OrderSummaryResponse(order.Id, order.CreatedAtUtc.ToLocalTime(), order.TotalAmount,
            order.Items.Select(oi => new OrderItemResponse(oi.ProductId, oi.Quantity, oi.UnitPrice)).ToList());
    }
}