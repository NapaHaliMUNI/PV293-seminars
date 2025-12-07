using Yestino.ProductCatalogContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.EventHandlers;

public static class CreateWarehouseProductOnProductCreatedHandler
{
    public static void Handle(ProductCreated domainEvent, WarehouseDbContext dbContext)
    {
        var warehouseProduct = new WarehouseProduct
        {
            Id = Guid.NewGuid(),
            ProductId = domainEvent.AggregateId,
            Quantity = 0
        };

        dbContext.WarehouseProducts.Add(warehouseProduct);
    }
}