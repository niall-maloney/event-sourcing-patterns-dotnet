using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_reserving")]
public record ExpectationReserving(string MatchingId, string ExpectationId) : IEvent;
