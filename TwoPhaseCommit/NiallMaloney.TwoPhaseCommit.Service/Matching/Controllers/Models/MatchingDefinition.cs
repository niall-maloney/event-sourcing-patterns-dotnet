namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;

public record MatchingDefinition(
    string ExpectationId,
    string PaymentId,
    string Iban,
    decimal Amount,
    string Reference
);
