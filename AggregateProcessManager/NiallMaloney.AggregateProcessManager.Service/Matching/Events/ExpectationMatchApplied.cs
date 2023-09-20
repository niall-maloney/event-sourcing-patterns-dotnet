using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_match_applied")]
public record ExpectationMatchApplied(string MatchingId, string ExpectationId, string PaymentId)
    : IEvent;
