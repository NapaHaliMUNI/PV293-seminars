using Yestino.Ordering.Application;
using Yestino.Ordering.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Ordering.Features.EventHandlers;

public static class UpdateStockOnStockAddedHandler
{
    public static void Handle(StockAdded domainEvent, OrderingDbContext dbContext)
    {
        var product = dbContext.ProductReadModels.FirstOrDefault(p => p.Id == domainEvent.ProductId);
        if (product != null)
        {
            // Since it's a record, need to remove and add new
            dbContext.ProductReadModels.Remove(product);
            var updated = product with { StockQuantity = product.StockQuantity + domainEvent.AddedQuantity };
            dbContext.ProductReadModels.Add(updated);
        }
    }
}