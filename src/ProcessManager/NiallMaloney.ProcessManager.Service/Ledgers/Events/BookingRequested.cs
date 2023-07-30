using NiallMaloney.EventSourcing;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Events;

[Event("ledgers.booking_requested")]
public record BookingRequested(string BookingId, string Ledger, decimal Amount) : IEvent;
