using MediatR;
using NiallMaloney.ProcessManager.Cassandra;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Queries;

public record SearchBookings
    (string? BookingId = null, string? Ledger = null, string? Status = null) : IRequest<IEnumerable<BookingRow>>;