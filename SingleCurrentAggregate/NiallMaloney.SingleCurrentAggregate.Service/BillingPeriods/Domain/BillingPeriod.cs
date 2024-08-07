using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Events;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Domain;

[Category("single_current_aggregate.billing_period")]
public class BillingPeriod : Aggregate
{
    private readonly Charges _charges = new();
    private bool _closed;
    private string _customerId = string.Empty;

    private bool _opened;

    public BillingPeriod()
    {
        When<BillingPeriodOpened>(Apply);
        When<BillingPeriodClosed>(Apply);
        When<ChargeAdded>(Apply);
        When<ChargeRemoved>(Apply);
    }

    public void Open(string customerId)
    {
        if (_opened)
        {
            return;
        }

        if (string.IsNullOrEmpty(customerId))
        {
            throw new InvalidOperationException();
        }

        RaiseEvent(new BillingPeriodOpened(Id, customerId));
    }

    public void Close()
    {
        if (_closed)
        {
            return;
        }

        RaiseEvent(new BillingPeriodClosed(Id, _customerId, _charges.GetTotalAmount()));
    }

    public void AddCharge(string chargeId, decimal amount)
    {
        if (_charges.Contains(chargeId) || _closed)
        {
            return;
        }

        var totalAmount = _charges.GetTotalAmount() + amount;
        RaiseEvent(new ChargeAdded(Id, chargeId, amount, totalAmount));
    }

    public void RemoveCharge(string chargeId)
    {
        if (!_charges.Contains(chargeId) || _closed)
        {
            return;
        }

        var amount = _charges.GetChargeAmount(chargeId);
        var totalAmount = _charges.GetTotalAmount() - amount;
        RaiseEvent(new ChargeRemoved(Id, chargeId, amount, totalAmount));
    }

    private void Apply(BillingPeriodOpened evnt)
    {
        _opened = true;
        _customerId = evnt.CustomerId;
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