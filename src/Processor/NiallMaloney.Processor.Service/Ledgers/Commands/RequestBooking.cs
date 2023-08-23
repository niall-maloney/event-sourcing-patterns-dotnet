using MediatR;

namespace NiallMaloney.Processor.Service.Ledgers.Commands;

public record RequestBooking(string BookingId, string Ledger, decimal Amount) : IRequest;
