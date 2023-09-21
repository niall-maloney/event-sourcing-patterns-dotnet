using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("aggregate_process_manager.payment_match_rejected")]
public record PaymentMatchRejected(string PaymentId, string MatchingId) : IEvent;
