using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_reserved")]
public record ExpectationReserved(string MatchingId, string ExpectationId) : IEvent;
