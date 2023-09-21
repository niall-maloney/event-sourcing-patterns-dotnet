using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.matching_failed")]
public record MatchingFailed(string MatchingId, string Reason) : IEvent;
