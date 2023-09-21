using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("aggregate_process_manager.payment_reserving")]
public record PaymentReserving(string MatchingId, string PaymentId) : IEvent;
