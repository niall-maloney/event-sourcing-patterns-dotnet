using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.payment_match_applied")]
public record PaymentMatchApplied(string MatchingId, string PaymentId, string ExpectationId) : IEvent;
