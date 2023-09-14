using NiallMaloney.EventSourcing;

namespace NiallMaloney.AggregateProcessManager.Service.Payments.Events;

[Event("aggregate_process_manager.payment_reserved")]
public record PaymentReserved(string PaymentId, string MatchingId) : IEvent;
