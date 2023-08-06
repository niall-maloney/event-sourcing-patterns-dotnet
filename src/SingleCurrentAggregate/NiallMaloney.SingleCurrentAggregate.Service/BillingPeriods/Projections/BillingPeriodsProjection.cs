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
        When<ChargeAdded>(Handle);
        When<ChargeRemoved>(Handle);
    }

    private async Task Handle(BillingPeriodOpened evnt, EventMetadata metadata)
    {
        var billingPeriod = await _repository.GetBillingPeriod(evnt.BillingPeriodId);
        if (billingPeriod is not null)
        {
            return;
        }

        await _repository.AddBillingPeriod(new BillingPeriodRow
        {
            BillingPeriodId = evnt.BillingPeriodId,
            Status = "Open",
            TotalAmount = 0,
            Version = metadata.StreamPosition
        });
    }

    private async Task Handle(BillingPeriodClosed evnt, EventMetadata metadata)
    {
        var billingPeriod = await _repository.GetBillingPeriod(evnt.BillingPeriodId);
        if (billingPeriod is null ||
            !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
        {
            return;
        }

        billingPeriod = billingPeriod with
        {
            Status = "Closed",
            TotalAmount = evnt.TotalAmount
        };

        await _repository.UpdateBillingPeriod(billingPeriod);
    }


    private async Task Handle(ChargeAdded evnt, EventMetadata metadata)
    {
        var billingPeriod = await _repository.GetBillingPeriod(evnt.BillingPeriodId);
        if (billingPeriod is null ||
            !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
        {
            return;
        }

        billingPeriod = billingPeriod with
        {
            TotalAmount = evnt.TotalAmount
        };
        await _repository.UpdateBillingPeriod(billingPeriod);
    }

    private async Task Handle(ChargeRemoved evnt, EventMetadata metadata)
    {
        var billingPeriod = await _repository.GetBillingPeriod(evnt.BillingPeriodId);
        if (billingPeriod is null ||
            !TryUpdateVersion(billingPeriod, metadata.StreamPosition, out billingPeriod))
        {
            return;
        }

        billingPeriod = billingPeriod with
        {
            TotalAmount = evnt.TotalAmount
        };
        await _repository.UpdateBillingPeriod(billingPeriod);
    }

    private bool TryUpdateVersion(
        BillingPeriodRow billingPeriod,
        ulong newVersion,
        out BillingPeriodRow newBillingPeriod)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = billingPeriod.Version;
        if (actualVersion >= newVersion)
        {
            newBillingPeriod = billingPeriod;
            return false;
        }
        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException($"Version mismatch, expected {expectedVersion} actual {actualVersion}");
        }
        newBillingPeriod = billingPeriod with
        {
            Version = newVersion
        };
        return true;
    }
}