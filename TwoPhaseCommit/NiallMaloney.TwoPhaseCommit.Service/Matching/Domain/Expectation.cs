namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Domain;

public record Expectation(
    string Id,
    string Iban,
    decimal Amount,
    string Reference,
    bool IsReserved = false,
    bool IsMatched = false
);