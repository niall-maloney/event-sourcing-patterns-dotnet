using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_reserving")]
public record ExpectationReserving(string MatchingId, string ExpectationId) : IEvent;
