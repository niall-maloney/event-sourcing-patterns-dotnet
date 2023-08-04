using NiallMaloney.EventSourcing;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("single_current_aggregate.charge_added")]
public record ChargeAdded(string BillingPeriodId, string ChargeId, decimal Amount, decimal TotalAmount) : IEvent;
