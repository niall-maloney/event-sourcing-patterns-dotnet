using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.matching_completed")]
public record MatchingCompleted(string MatchingId) : IEvent;