using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_reservation_rejected")]
public record ExpectationReservationRejected(
    string MatchingId,
    string ExpectationId,
    string PaymentId
) : IEvent;
