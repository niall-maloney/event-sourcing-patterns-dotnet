using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.expectation_match_applying")]
public record ExpectationMatchApplying(string MatchingId, string ExpectationId, string PaymentId)
    : IEvent;
