using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("two_phase_commit.payment_match_rejected")]
public record PaymentMatchRejected(string PaymentId, string MatchingId) : IEvent;
