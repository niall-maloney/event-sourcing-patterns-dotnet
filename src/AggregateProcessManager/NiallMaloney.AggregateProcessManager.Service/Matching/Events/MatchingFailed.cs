using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.matching_failed")]
public record MatchingFailed(string MatchingId) : IEvent;
