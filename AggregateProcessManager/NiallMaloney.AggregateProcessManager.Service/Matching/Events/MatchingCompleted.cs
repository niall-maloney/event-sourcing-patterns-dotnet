using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.matching_completed")]
public record MatchingCompleted(string MatchingId) : IEvent;
