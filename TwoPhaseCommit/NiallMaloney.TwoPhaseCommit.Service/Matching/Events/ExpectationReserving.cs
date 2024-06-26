using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.expectation_reserving")]
public record ExpectationReserving(string MatchingId, string ExpectationId) : IEvent;