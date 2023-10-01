using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("two_phase_commit.payment_released")]
public record PaymentReleased(string PaymentId, string MatchingId) : IEvent;
