using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("aggregate_process_manager.expectation_match_rejected")]
public record ExpectationMatchRejected(string ExpectationId, string MatchingId) : IEvent;
