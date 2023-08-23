using MediatR;

namespace NiallMaloney.Processor.Service.Ledgers.Commands;

public record RejectBooking(string BookingId, decimal Balance) : IRequest;
