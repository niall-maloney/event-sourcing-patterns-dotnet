using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.ProcessManager.Service.Ledgers.Events;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Domain;

[Category("ledgers.booking")]
public class Booking : Aggregate
{
    private bool _requested;
    private bool _committed;
    private bool _rejected;

    private string Ledger { get; set; }
    private decimal Amount { get; set; }

    public Booking()
    {
        When<BookingRequested>(Apply);
        When<BookingCommitted>(Apply);
        When<BookingRejected>(Apply);
    }

    public void Request(string ledger, decimal amount)
    {
        if (_requested)
        {
            return;
        }

        RaiseEvent(new BookingRequested(Id, ledger, amount));
    }

    public void Commit(decimal balance)
    {
        if (_requested == false)
        {
            throw new InvalidOperationException();
        }

        if (_committed)
        {
            return;
        }

        RaiseEvent(new BookingCommitted(Id, Ledger, Amount, balance));
    }

    public void Reject(decimal balance)
    {
        if (_requested == false || _committed)
        {
            throw new InvalidOperationException();
        }

        if (_rejected)
        {
            return;
        }

        RaiseEvent(new BookingRejected(Id, Ledger, Amount, balance));
    }

    private void Apply(BookingRequested evnt)
    {
        _requested = true;
        Ledger = evnt.Ledger;
        Amount = evnt.Amount;
    }


    private void Apply(BookingCommitted evnt)
    {
        _committed = true;
    }

    private void Apply(BookingRejected evnt)
    {
        _rejected = true;
    }
}
