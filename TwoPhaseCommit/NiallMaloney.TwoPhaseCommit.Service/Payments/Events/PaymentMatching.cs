using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("two_phase_commit.payment_matching")]
public record PaymentMatching(string PaymentId, string MatchingId) : IEvent;
