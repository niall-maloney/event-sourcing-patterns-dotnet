using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.SingleCurrentAggregate.Cassandra;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Projections;

[SubscriberName("ChargesProjection")]
[Subscription("$ce-single_current_aggregate.billing_period")]
public class ChargesProjection : Projection
{
    private readonly IChargesRepository _repository;

    public ChargesProjection(ILogger<BillingPeriodsProjection> logger, IChargesRepository repository)
    {
        _repository = repository;

        When<ChargeAdded>(Handle);
        When<ChargeRemoved>(Handle);
    }

    private async Task Handle(ChargeAdded evnt, EventMetadata metadata)
    {
        var charge = await _repository.GetCharge(evnt.ChargeId);
        if (charge is not null)
        {
            return;
        }
        await _repository.AddCharge(new ChargeRow
        {
            ChargeId = evnt.ChargeId,
            BillingPeriodId = evnt.BillingPeriodId,
            Amount = evnt.Amount,
            Status = "Charged",
            Version = metadata.StreamPosition
        });
    }

    private async Task Handle(ChargeRemoved evnt, EventMetadata metadata)
    {
        var charge = await _repository.GetCharge(evnt.ChargeId);
        if (charge is null || !TryUpdateVersion(charge, metadata.StreamPosition, out charge))
        {
            return;
        }
        charge = charge with
        {
            Status = "Removed"
        };
        await _repository.UpdateCharge(charge);
    }

    private bool TryUpdateVersion(
        ChargeRow charge,
        ulong newVersion,
        out ChargeRow newCharge)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = charge.Version;
        if (actualVersion >= newVersion)
        {
            newCharge = charge;
            return false;
        }
        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException($"Version mismatch, expected {expectedVersion} actual {actualVersion}");
        }
        newCharge = charge with
        {
            Version = newVersion
        };
        return true;
    }
}
