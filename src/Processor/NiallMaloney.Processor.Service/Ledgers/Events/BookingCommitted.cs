using NiallMaloney.EventSourcing;

namespace NiallMaloney.Processor.Service.Ledgers.Events;

[Event("processor.booking_committed")]
public record BookingCommitted(string BookingId, string Ledger, decimal Amount, decimal Balance) : IEvent;
