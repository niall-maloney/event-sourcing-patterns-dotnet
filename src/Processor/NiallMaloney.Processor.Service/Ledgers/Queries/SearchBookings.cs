using MediatR;
using NiallMaloney.Processor.Cassandra;

namespace NiallMaloney.Processor.Service.Ledgers.Queries;

public record SearchBookings
    (string? BookingId = null, string? Ledger = null, string? Status = null) : IRequest<IEnumerable<BookingRow>>;
