using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Events;

[Event("two_phase_commit.matching_started")]
public record MatchingStarted(
    string MatchingId,
    string ExpectationId,
    string PaymentId,
    string Iban,
    decimal Amount,
    string Reference
) : IEvent;