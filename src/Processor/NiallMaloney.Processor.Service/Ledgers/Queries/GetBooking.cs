using MediatR;
using NiallMaloney.Processor.Cassandra;

namespace NiallMaloney.Processor.Service.Ledgers.Queries;

public record GetBooking(string BookingId) : IRequest<BookingRow?>;
