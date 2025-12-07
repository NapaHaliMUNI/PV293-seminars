using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Warehouse.Features.AddStock;

public static class AddStockEndpoint
{
    public static WarehouseProduct? Before(AddStockCommand command, WarehouseDbContext dbContext)
    {
        return dbContext.WarehouseProducts.FirstOrDefault(x => x.ProductId == command.ProductId);
    }

    [WolverinePost("/warehouse/add-stock")]
    public static (IResult, IStorageAction<WarehouseProduct>, StockAdded?) AddStock([NotBody] WarehouseProduct? warehouseProduct,
        AddStockCommand command)
    {
        if (warehouseProduct is null)
        {
            return (
                Results.BadRequest("Product not found in warehouse"),
                Storage.Nothing<WarehouseProduct>(),
                null
            );
        }

        if (command.Quantity <= 0)
        {
            return (
                Results.BadRequest("Quantity must be positive"),
                Storage.Nothing<WarehouseProduct>(),
                null
            );
        }

        warehouseProduct.Quantity += command.Quantity;

        return (
            Results.Ok(),
            Storage.Update(warehouseProduct),
            new StockAdded(warehouseProduct.ProductId, command.Quantity)
        );
    }
}