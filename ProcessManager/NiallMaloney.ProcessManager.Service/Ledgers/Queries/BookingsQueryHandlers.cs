using MediatR;
using NiallMaloney.ProcessManager.Cassandra;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Queries;

public class BookingsQueryHandlers : IRequestHandler<GetBooking, BookingRow?>,
    IRequestHandler<SearchBookings, IEnumerable<BookingRow>>
{
    private readonly IBookingsRepository _repository;

    public BookingsQueryHandlers(IBookingsRepository repository)
    {
        _repository = repository;
    }

    public Task<BookingRow?> Handle(GetBooking request, CancellationToken cancellationToken) =>
        _repository.GetBooking(request.BookingId);

    public Task<IEnumerable<BookingRow>> Handle(SearchBookings request, CancellationToken cancellationToken) =>
        _repository.SearchBookings(request.BookingId, request.Ledger, request.Status);
}
