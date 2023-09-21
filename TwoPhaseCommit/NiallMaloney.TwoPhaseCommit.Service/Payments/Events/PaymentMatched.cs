using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("two_phase_commit.payment_matched")]
public record PaymentMatched(string PaymentId, string ExpectationId, string MatchingId) : IEvent;
