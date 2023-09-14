using NiallMaloney.EventSourcing;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Events;

[Event("processor.booking_committed")]
public record BookingCommitted(string BookingId, string Ledger, decimal Amount, decimal Balance) : IEvent;
