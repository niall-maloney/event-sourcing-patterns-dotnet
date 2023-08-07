using MediatR;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Commands;

public record RejectBooking(string BookingId, decimal Balance) : IRequest;