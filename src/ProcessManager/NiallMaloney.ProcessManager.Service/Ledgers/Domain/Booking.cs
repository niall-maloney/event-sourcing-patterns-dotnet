using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.ProcessManager.Service.Ledgers.Events;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Domain;

[Category("process_manager.booking")]
public class Booking : Aggregate
{
    private bool _committed;
    private bool _rejected;
    private bool _requested;

    public Booking()
    {
        When<BookingRequested>(Apply);
        When<BookingCommitted>(Apply);
        When<BookingRejected>(Apply);
    }

    private string? Ledger { get; set; }
    private decimal Amount { get; set; }

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
        //todo: log error
        if (_requested == false)
        {
            return;
        }

        if (_committed)
        {
            return;
        }

        RaiseEvent(new BookingCommitted(Id, Ledger!, Amount, balance));
    }

    public void Reject(decimal balance)
    {
        //todo: log error
        if (_requested == false || _committed)
        {
            return;
        }

        if (_rejected)
        {
            return;
        }

        RaiseEvent(new BookingRejected(Id, Ledger!, Amount, balance));
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