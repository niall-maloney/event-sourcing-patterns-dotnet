using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Events;

[Event("aggregate_process_manager.payment_released")]
public record PaymentReleased(string PaymentId, string MatchingId) : IEvent;
