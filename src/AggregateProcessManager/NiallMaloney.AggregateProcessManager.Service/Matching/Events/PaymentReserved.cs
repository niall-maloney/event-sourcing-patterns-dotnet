using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.payment_reserved")]
public record PaymentReserved(string MatchingId, string PaymentId) : IEvent;
