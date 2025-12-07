using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.EventHandlers;

public static class SubtractStockOnOrderShippedHandler
{
    public static void Handle(OrderShipped domainEvent, WarehouseDbContext dbContext)
    {
        foreach (var item in domainEvent.Items)
        {
            var warehouseProduct = dbContext.WarehouseProducts.FirstOrDefault(wp => wp.ProductId == item.ProductId);
            if (warehouseProduct != null)
            {
                warehouseProduct.Quantity -= item.Quantity;
                if (warehouseProduct.Quantity < 0)
                {
                    warehouseProduct.Quantity = 0;
                }
            }
        }
    }
}