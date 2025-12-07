using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.Commands.CreateOrder;

public static class CreateOrderCommandHandler
{
    [WolverinePost("/orders")]
    public static IResult Handle(CreateOrderCommand command, OrderingDbContext dbContext)
    {
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var products = dbContext.ProductReadModels.Where(p => productIds.Contains(p.Id)).ToDictionary(p => p.Id);

        // Check all products exist
        if (products.Count != productIds.Distinct().Count())
        {
            return Results.BadRequest("Some products do not exist");
        }

        // Check stock
        foreach (var item in command.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product) || product.StockQuantity < item.Quantity)
            {
                return Results.BadRequest($"Insufficient stock for product {item.ProductId}");
            }
        }

        // Reserve stock
        foreach (var item in command.Items)
        {
            var product = products[item.ProductId];
            dbContext.ProductReadModels.Remove(product);
            var updated = product with { StockQuantity = product.StockQuantity - item.Quantity };
            dbContext.ProductReadModels.Add(updated);
            products[item.ProductId] = updated; // update dict for CreateOrderItemModel
        }

        var order = Order.Create(
            command.CustomerAddress,
            command.Items
                .Select(i => new CreateOrderItemModel(i.ProductId, i.Quantity, products[i.ProductId].Price, products[i.ProductId].Name))
                .ToList()
        );

        dbContext.Orders.Add(order);

        return Results.Ok(order.Id);
    }
}