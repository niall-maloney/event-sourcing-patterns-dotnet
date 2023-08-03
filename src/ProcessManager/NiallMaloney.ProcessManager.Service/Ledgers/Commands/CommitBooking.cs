using MediatR;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Commands;

public record CommitBooking(string BookingId, decimal Balance) : IRequest;