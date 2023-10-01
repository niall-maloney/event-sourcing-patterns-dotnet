using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.payment_reserving")]
public record PaymentReserving(string MatchingId, string PaymentId) : IEvent;
