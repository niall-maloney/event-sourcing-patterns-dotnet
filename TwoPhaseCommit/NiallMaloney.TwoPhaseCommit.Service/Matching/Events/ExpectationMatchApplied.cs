using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.expectation_match_applied")]
public record ExpectationMatchApplied(string MatchingId, string ExpectationId, string PaymentId)
    : IEvent;