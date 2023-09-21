using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.payment_reserved")]
public record PaymentReserved(string MatchingId, string PaymentId) : IEvent;
