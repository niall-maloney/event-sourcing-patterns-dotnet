using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.payment_reservation_rejected")]
public record PaymentReservationRejected(string MatchingId, string PaymentId) : IEvent;
