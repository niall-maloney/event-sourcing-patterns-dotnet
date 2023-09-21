using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.payment_match_applying")]
public record PaymentMatchApplying(string MatchingId, string PaymentId, string ExpectationId)
    : IEvent;
