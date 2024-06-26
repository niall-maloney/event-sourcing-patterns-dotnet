using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("two_phase_commit.expectation_matched")]
public record ExpectationMatched(string ExpectationId, string PaymentId, string MatchingId)
    : IEvent;