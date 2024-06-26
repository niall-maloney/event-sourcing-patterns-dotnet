using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.expectation_reservation_rejected")]
public record ExpectationReservationRejected(
    string MatchingId,
    string ExpectationId,
    string PaymentId
) : IEvent;