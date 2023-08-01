using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

[Category("single_current_aggregate.billing_period")]
public class BillingPeriod : Aggregate
{
    private bool _opened;
    private bool _closed;

    private readonly Charges _charges = new();

    public BillingPeriod()
    {
        When<BillingPeriodOpened>(Apply);
        When<BillingPeriodClosed>(Apply);
        When<ChargeAdded>(Apply);
        When<ChargeRemoved>(Apply);
    }

    public void Open()
    {
        if (_opened)
        {
            return;
        }

        RaiseEvent(new BillingPeriodOpened(Id));
    }

    public void Close()
    {
        if (_closed)
        {
            return;
        }

        RaiseEvent(new BillingPeriodClosed(Id));
    }

    public void AddCharge(string chargeId, decimal amount)
    {
        if (_charges.Contains(chargeId))
        {
            return;
        }

        RaiseEvent(new ChargeAdded(Id, chargeId, amount));
    }

    public void RemoveCharge(string chargeId)
    {
        if (!_charges.Contains(chargeId))
        {
            return;
        }

        RaiseEvent(new ChargeRemoved(Id, chargeId, _charges.GetAmount(chargeId)));
    }

    private void Apply(BillingPeriodOpened evnt)
    {
        _opened = true;
    }

    private void Apply(BillingPeriodClosed evnt)
    {
        _closed = true;
    }

    private void Apply(ChargeAdded evnt)
    {
        _charges.Add(evnt.ChargeId, evnt.Amount);
    }

    private void Apply(ChargeRemoved evnt)
    {
        _charges.Remove(evnt.ChargeId);
    }
}
