using NiallMaloney.EventSourcing;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Events;

[Event("ledgers.booking_rejected")]
public record BookingRejected(string BookingId, string Ledger, decimal Amount, decimal Balance) : IEvent;
