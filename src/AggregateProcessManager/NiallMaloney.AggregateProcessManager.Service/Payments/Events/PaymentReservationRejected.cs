using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Events;

[Event("aggregate_process_manager.payment_reservation_rejected")]
public record PaymentMatchRejected(string PaymentId, string MatchingId) : IEvent;
