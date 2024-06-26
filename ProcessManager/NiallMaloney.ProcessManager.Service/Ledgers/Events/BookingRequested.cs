using NiallMaloney.EventSourcing;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Events;

[Event("process_manager.booking_requested")]
public record BookingRequested(string BookingId, string Ledger, decimal Amount) : IEvent;