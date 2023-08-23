using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.Processor.Cassandra;
using NiallMaloney.Processor.Service.Ledgers.Commands;
using NiallMaloney.Processor.Service.Ledgers.Events;

namespace NiallMaloney.Processor.Service.Ledgers;

[SubscriberName("LedgersProcessor")]
[Subscription("$ce-processor.booking")]
public class LedgersProcessor : SubscriberBase
{
    private readonly ILogger<LedgersProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly ILedgersRepository _repository;

    public LedgersProcessor(
        ILogger<LedgersProcessor> logger,
        ILedgersRepository repository,
        IMediator mediator)
    {
        _logger = logger;
        _repository = repository;
        _mediator = mediator;

        When<BookingRequested>(Handle);
        When<BookingCommitted>(Handle);
    }

    private async Task Handle(BookingRequested evnt, EventMetadata metadata)
    {
        var bookingId = evnt.BookingId;
        var ledger = evnt.Ledger;
        var amount = evnt.Amount;

        var row = await _repository.GetLedger(ledger);

        var newStreamPosition = metadata.AggregatedStreamPosition;
        var lastStreamPosition = row?.LastStreamPosition;
        if (lastStreamPosition >= newStreamPosition)
        {
            _logger.LogInformation("Event already processed. Last stream position: ({0}), Event stream position: ({1})",
                lastStreamPosition, newStreamPosition);
            return;
        }

        var currentPendingBalance = row?.PendingAmount ?? 0;
        var currentCommittedBalance = row?.CommittedAmount ?? 0;

        var newPendingBalance = currentPendingBalance + amount;
        var newBalance = currentCommittedBalance + newPendingBalance;
        if (newBalance >= 0)
        {
            await _repository.UpdatePendingBalance(ledger, newPendingBalance, newStreamPosition, lastStreamPosition);
            await _mediator.Send(new CommitBooking(bookingId, newBalance));
        }
        else
        {
            var currentBalance = currentCommittedBalance + currentPendingBalance;
            await _mediator.Send(new RejectBooking(bookingId, currentBalance));
        }
    }

    private async Task Handle(BookingCommitted evnt, EventMetadata metadata)
    {
        var ledger = evnt.Ledger;
        var amount = evnt.Amount;

        var row = await _repository.GetLedger(ledger);

        var newStreamPosition = metadata.AggregatedStreamPosition;
        var lastStreamPosition = row?.LastStreamPosition;
        if (lastStreamPosition >= newStreamPosition)
        {
            _logger.LogInformation("Event already processed. Last stream position: ({0}), Event stream position: ({1})",
                lastStreamPosition, newStreamPosition);
            return;
        }

        var currentPendingBalance = row?.PendingAmount ?? 0;
        var currentCommittedBalance = row?.CommittedAmount ?? 0;

        var newPendingBalance = currentPendingBalance - amount;
        var newCommittedBalance = currentCommittedBalance + amount;

        await _repository.UpdateBalance(ledger, newPendingBalance, newCommittedBalance, newStreamPosition,
            lastStreamPosition);
    }
}
