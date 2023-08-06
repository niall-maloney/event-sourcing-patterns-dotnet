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
        When<BookingCommitted>(Handle);
    }

    //todo: idempotency to protect against replayed events
    private async Task Handle(BookingRequested evnt, EventMetadata metadata)
    {
        var bookingId = evnt.BookingId;

        var (success, balance) = await UpdateBalance(evnt);
        if (success)
        {
            await _mediator.Send(new CommitBooking(bookingId, balance));
        }
        else
        {
            await _mediator.Send(new RejectBooking(bookingId, balance));
        }
    }

    private async Task<(bool, decimal)> UpdateBalance(BookingRequested evnt)
    {
        var ledger = evnt.Ledger;
        var amount = evnt.Amount;

        var (currentPendingBalance, currentCommittedBalance) = await _repository.GetBalance(ledger);
        var pendingBalance = currentPendingBalance ?? 0;
        var committedBalance = currentCommittedBalance ?? 0;

        var updatedPendingBalance = pendingBalance + amount;
        var updatedBalance = committedBalance + updatedPendingBalance;
        if (updatedBalance < 0)
        {
            return (false, committedBalance + pendingBalance);
        }

        await _repository.UpdateBalance(ledger, updatedPendingBalance, currentPendingBalance, committedBalance,
            currentCommittedBalance);

        return (true, updatedBalance);
    }

    private async Task Handle(BookingCommitted evnt, EventMetadata metadata)
    {
        var ledger = evnt.Ledger;
        var amount = evnt.Amount;

        var (currentPendingBalance, currentCommittedBalance) = await _repository.GetBalance(ledger);
        var pendingBalance = currentPendingBalance ?? 0;
        var committedBalance = currentCommittedBalance ?? 0;

        var updatedPendingBalance = pendingBalance - amount;
        var updatedCommittedBalance = committedBalance + amount;

        await _repository.UpdateBalance(ledger, updatedPendingBalance, currentPendingBalance, updatedCommittedBalance,
            currentCommittedBalance);
    }
}
