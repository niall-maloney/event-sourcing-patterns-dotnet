using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("aggregate_process_manager.expectation_matching")]
public record ExpectationMatching(string ExpectationId, string MatchingId) : IEvent;
