using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("two_phase_commit.expectation_matching")]
public record ExpectationMatching(string ExpectationId, string MatchingId) : IEvent;