using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.payment_reservation_rejected")]
public record PaymentReservationRejected(string MatchingId, string PaymentId) : IEvent;