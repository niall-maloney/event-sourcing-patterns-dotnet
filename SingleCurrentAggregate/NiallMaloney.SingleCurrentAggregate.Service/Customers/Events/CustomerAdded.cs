using NiallMaloney.EventSourcing;

namespace NiallMaloney.SingleCurrentAggregate.Service.Customers.Events;

[Event("single_current_aggregate.customer_added")]
public record CustomerAdded(string CustomerId) : IEvent;
