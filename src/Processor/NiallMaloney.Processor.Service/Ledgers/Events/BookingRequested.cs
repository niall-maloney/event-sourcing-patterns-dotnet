using NiallMaloney.EventSourcing;

namespace NiallMaloney.Processor.Service.Ledgers.Events;

[Event("processor.booking_requested")]
public record BookingRequested(string BookingId, string Ledger, decimal Amount) : IEvent;
