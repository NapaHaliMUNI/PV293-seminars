using Yestino.Common.Domain;

namespace Yestino.Warehouse.Entities;

public class WarehouseProduct
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}