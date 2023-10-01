using NiallMaloney.EventSourcing;

namespace NiallMaloney.TwoPhaseCommit.Service.Expectations.Events;

[Event("two_phase_commit.expectation_created")]
public record ExpectationCreated(
    string ExpectationId,
    string Iban,
    decimal Amount,
    string Reference
) : IEvent;
