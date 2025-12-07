using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;
using Yestino.OrderingContracts.DomainEvents;

namespace Yestino.Ordering.Features.Commands.ShipOrder;

public static class ShipOrderCommandHandler
{
    [WolverinePost("/orders/ship")]
    public static IResult Handle(ShipOrderCommand command, OrderingDbContext dbContext)
    {
        var order = dbContext.Orders.FirstOrDefault(o => o.Id == command.OrderId);
        if (order == null)
        {
            return Results.NotFound("Order not found");
        }

        if (order.Status != OrderStatus.Created && order.Status != OrderStatus.Processing)
        {
            return Results.BadRequest("Order cannot be shipped");
        }

        order.Status = OrderStatus.Shipped;

        var eventItems = order.Items.Select(i => new OrderShippedItem(i.ProductId, i.Quantity)).ToList();
        order.RaiseDomainEvent(new OrderShipped(order.Id, eventItems));

        return Results.Ok();
    }
}