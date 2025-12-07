using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record OrderShipped(Guid AggregateId, ICollection<OrderShippedItem> Items) : DomainEvent(AggregateId);

public record OrderShippedItem(Guid ProductId, int Quantity);