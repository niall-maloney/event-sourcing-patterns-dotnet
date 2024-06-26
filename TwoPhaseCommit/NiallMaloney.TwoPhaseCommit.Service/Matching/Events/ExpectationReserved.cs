using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.expectation_reserved")]
public record ExpectationReserved(string MatchingId, string ExpectationId) : IEvent;