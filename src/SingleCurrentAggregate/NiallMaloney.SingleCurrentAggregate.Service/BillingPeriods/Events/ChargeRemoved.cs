using NiallMaloney.EventSourcing;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

[Event("single_current_aggregate.charge_removed")]
public record ChargeRemoved(string BillingPeriodId, string ChargeId, decimal Amount, decimal TotalAmount) : IEvent;