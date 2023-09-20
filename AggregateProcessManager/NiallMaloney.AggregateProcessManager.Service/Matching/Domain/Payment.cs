namespace NiallMaloney.AggregateProcessManager.Service.Matching.Domain;

public record Payment(
    string Id,
    string Iban,
    decimal Amount,
    string Reference,
    bool IsReserved = false,
    bool IsMatched = false
);
