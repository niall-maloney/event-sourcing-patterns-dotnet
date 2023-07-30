using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.ProcessManager.Service.Ledgers.Commands;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Domain;

public class BookingHandlers : IRequestHandler<RequestBooking>, IRequestHandler<CommitBooking>,
    IRequestHandler<RejectBooking>
{
    private readonly AggregateRepository _aggregateRepository;

    public BookingHandlers(AggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    public async Task Handle(RequestBooking request, CancellationToken cancellationToken)
    {
        var booking = await _aggregateRepository.LoadAggregate<Booking>(request.BookingId);
        booking.Request(request.Ledger, request.Amount);
        await _aggregateRepository.SaveAggregate(booking);
    }

    public async Task Handle(CommitBooking request, CancellationToken cancellationToken)
    {
        var booking = await _aggregateRepository.LoadAggregate<Booking>(request.BookingId);
        booking.Commit(request.Balance);
        await _aggregateRepository.SaveAggregate(booking);
    }

    public async Task Handle(RejectBooking request, CancellationToken cancellationToken)
    {
        var booking = await _aggregateRepository.LoadAggregate<Booking>(request.BookingId);
        booking.Reject(request.Balance);
        await _aggregateRepository.SaveAggregate(booking);
    }
}
