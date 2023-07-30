using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.ProcessManager.Service.Ledgers.Commands;
using NiallMaloney.ProcessManager.Service.Ledgers.Events;
using NiallMaloney.ProcessManager.Service.Ledgers.Repositories;

namespace NiallMaloney.ProcessManager.Service.Ledgers;

[SubscriberName("LedgersProcessManager")]
[Subscription("$ce-ledgers.booking")]
public class LedgersProcessManager : SubscriberBase
{
    private readonly IBalanceRepository _repository;
    private readonly IMediator _mediator;

    public LedgersProcessManager(IBalanceRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
        When<BookingRequested>(Handle);
    }

    private async Task Handle(BookingRequested evnt, EventMetadata metadata)
    {
        var bookingId = evnt.BookingId;
        var (success, balance) = await _repository.UpdateBalance(evnt.Ledger, evnt.Amount);
        if (success)
        {
            await _mediator.Send(new CommitBooking(bookingId, balance));
        }
        else
        {
            await _mediator.Send(new RejectBooking(bookingId, balance));
        }
    }
}
