using MediatR;

namespace NiallMaloney.Processor.Service.Ledgers.Commands;

public record CommitBooking(string BookingId, decimal Balance) : IRequest;
