using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.SingleCurrentAggregate.Cassandra;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Projections;

[SubscriberName("BillingPeriodsProjection")]
[Subscription("$ce-single_current_aggregate.billing_period")]
public class BillingPeriodsProjection : Projection
{
    private readonly IBillingPeriodsRepository _repository;

    public BillingPeriodsProjection(ILogger<BillingPeriodsProjection> logger, IBillingPeriodsRepository repository)
    {
        _repository = repository;

        When<BillingPeriodOpened>(Handle);
        When<BillingPeriodClosed>(Handle);
    }

    private async Task Handle(BillingPeriodOpened evnt, EventMetadata metadata)
    {
        await _repository.AddBillingPeriod(new BillingPeriodRow(evnt.BillingPeriodId, "Open"));
    }

    private async Task Handle(BillingPeriodClosed evnt, EventMetadata metadata)
    {
        await _repository.UpdateBillingPeriod(new BillingPeriodRow(evnt.BillingPeriodId, "Closed"));
    }
}
