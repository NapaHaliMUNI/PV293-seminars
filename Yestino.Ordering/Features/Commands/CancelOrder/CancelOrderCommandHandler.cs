using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.Commands.CancelOrder;

public static class CancelOrderCommandHandler
{
    [WolverinePost("/orders/cancel")]
    public static IResult Handle(CancelOrderCommand command, OrderingDbContext dbContext)
    {
        var order = dbContext.Orders.FirstOrDefault(o => o.Id == command.OrderId);
        if (order == null)
        {
            return Results.NotFound("Order not found");
        }

        if (order.Status != OrderStatus.Created && order.Status != OrderStatus.Processing)
        {
            return Results.BadRequest("Order cannot be cancelled");
        }

        order.Status = OrderStatus.Cancelled;

        // Release stock
        foreach (var item in order.Items)
        {
            var product = dbContext.ProductReadModels.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                dbContext.ProductReadModels.Remove(product);
                var updated = product with { StockQuantity = product.StockQuantity + item.Quantity };
                dbContext.ProductReadModels.Add(updated);
            }
        }

        return Results.Ok();
    }
}