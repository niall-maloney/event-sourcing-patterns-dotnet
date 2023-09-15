using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Matching.Events;

[Event("aggregate_process_manager.expectation_reservation_rejected")]
public record ExpectationReservationRejected(string MatchingId, string ExpectationId, string PaymentId) : IEvent;
