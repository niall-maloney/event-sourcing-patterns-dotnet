using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.payment_reserving")]
public record PaymentReserving(string MatchingId, string PaymentId) : IEvent;
