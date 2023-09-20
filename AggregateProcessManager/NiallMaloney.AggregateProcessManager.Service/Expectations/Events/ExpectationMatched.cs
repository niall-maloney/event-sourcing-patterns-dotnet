using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Events;

[Event("aggregate_process_manager.expectation_matched")]
public record ExpectationMatched(string ExpectationId, string PaymentId, string MatchingId)
    : IEvent;
