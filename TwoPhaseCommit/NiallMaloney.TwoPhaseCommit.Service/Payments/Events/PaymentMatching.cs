using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("aggregate_process_manager.payment_matching")]
public record PaymentMatching(string PaymentId, string MatchingId) : IEvent;
