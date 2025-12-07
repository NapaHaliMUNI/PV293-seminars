namespace Yestino.Warehouse.Features.AddStock;

public record AddStockCommand(Guid ProductId, int Quantity);