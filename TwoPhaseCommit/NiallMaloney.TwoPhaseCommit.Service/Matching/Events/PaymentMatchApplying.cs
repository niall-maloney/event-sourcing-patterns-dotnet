using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.payment_match_applying")]
public record PaymentMatchApplying(string MatchingId, string PaymentId, string ExpectationId)
    : IEvent;
