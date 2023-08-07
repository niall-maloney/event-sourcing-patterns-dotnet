using MediatR;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Commands;

public record RequestBooking(string BookingId, string Ledger, decimal Amount) : IRequest;