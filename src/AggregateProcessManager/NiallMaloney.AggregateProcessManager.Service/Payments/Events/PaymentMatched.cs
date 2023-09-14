using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Events;

[Event("aggregate_process_manager.payment_matched")]
public record PaymentMatched(string PaymentId, string ExpectationId, string MatchingId) : IEvent;
