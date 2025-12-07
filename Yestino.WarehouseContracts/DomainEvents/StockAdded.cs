using Yestino.Common.Domain;

namespace Yestino.WarehouseContracts.DomainEvents;

public record StockAdded(Guid ProductId, int AddedQuantity) : DomainEvent(Guid.NewGuid());