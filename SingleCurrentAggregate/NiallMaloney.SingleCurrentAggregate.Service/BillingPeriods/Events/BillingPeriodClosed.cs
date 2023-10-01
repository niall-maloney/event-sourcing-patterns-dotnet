using NiallMaloney.EventSourcing;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("single_current_aggregate.billing_period_closed")]
public record BillingPeriodClosed(string BillingPeriodId, string CustomerId, decimal TotalAmount)
    : IEvent;
