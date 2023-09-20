using NiallMaloney.EventSourcing;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("single_current_aggregate.billing_period_opened")]
public record BillingPeriodOpened(string BillingPeriodId, string CustomerId) : IEvent;
