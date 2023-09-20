using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_match_applying")]
public record ExpectationMatchApplying(string MatchingId, string ExpectationId, string PaymentId)
    : IEvent;
