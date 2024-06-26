using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("two_phase_commit.expectation_match_rejected")]
public record ExpectationMatchRejected(string ExpectationId, string MatchingId) : IEvent;