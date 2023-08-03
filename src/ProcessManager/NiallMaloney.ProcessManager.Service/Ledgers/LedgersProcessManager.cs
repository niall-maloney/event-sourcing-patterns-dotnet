using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.ProcessManager.Cassandra;
using NiallMaloney.ProcessManager.Service.Ledgers.Commands;
using NiallMaloney.ProcessManager.Service.Ledgers.Events;

namespace NiallMaloney.ProcessManager.Service.Ledgers;

[SubscriberName("LedgersProcessManager")]
[Subscription("$ce-process_manager.booking")]
public class LedgersProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;
    private readonly ILedgersRepository _repository;

    public LedgersProcessManager(ILedgersRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
        When<BookingRequested>(Handle);
    }

    private async Task Handle(BookingRequested evnt, EventMetadata metadata)
    {
        var bookingId = evnt.BookingId;

        var (success, balance) = await UpdateBalance(evnt);
        if (success)
            await _mediator.Send(new CommitBooking(bookingId, balance));
        else
            await _mediator.Send(new RejectBooking(bookingId, balance));
    }

    private async Task<(bool, decimal)> UpdateBalance(BookingRequested evnt)
    {
        var ledger = evnt.Ledger;
        var amount = evnt.Amount;

        var currentBalance = await _repository.GetBalance(ledger);
        var balance = currentBalance ?? 0;

        var updatedBalance = balance + amount;
        if (updatedBalance < 0) return (false, balance);

        return await _repository.UpdateBalance(ledger, updatedBalance, currentBalance);
    }
}