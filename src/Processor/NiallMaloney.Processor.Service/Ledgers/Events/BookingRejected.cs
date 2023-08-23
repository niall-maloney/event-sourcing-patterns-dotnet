using NiallMaloney.EventSourcing;

namespace NiallMaloney.Processor.Service.Ledgers.Events;

[Event("processor.booking_rejected")]
public record BookingRejected(string BookingId, string Ledger, decimal Amount, decimal Balance) : IEvent;
