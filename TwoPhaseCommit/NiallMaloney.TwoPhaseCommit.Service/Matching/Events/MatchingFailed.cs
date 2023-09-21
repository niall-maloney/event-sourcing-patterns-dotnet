using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.matching_failed")]
public record MatchingFailed(string MatchingId, string Reason) : IEvent;
