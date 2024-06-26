namespace NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

public record MatchingManagerRow
{
    public string MatchingId { get; init; } = string.Empty;
    public string PaymentId { get; init; } = string.Empty;
    public string ExpectationId { get; init; } = string.Empty;
    public string Iban { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public ulong Version { get; init; }
}