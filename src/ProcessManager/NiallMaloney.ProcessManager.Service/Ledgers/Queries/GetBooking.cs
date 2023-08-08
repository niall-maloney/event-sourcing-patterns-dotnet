using MediatR;
using NiallMaloney.ProcessManager.Cassandra;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Queries;

public record GetBooking(string BookingId) : IRequest<BookingRow?>;