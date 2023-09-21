using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_match_applying")]
public record ExpectationMatchApplying(string MatchingId, string ExpectationId, string PaymentId)
    : IEvent;
