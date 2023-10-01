namespace NiallMaloney.TwoPhaseCommit.Cassandra;

public record MatchingManagerRow
{
    public string MatchingId { get; init; }
    public string PaymentId { get; init; }
    public string ExpectationId { get; init; }
    public string Iban { get; init; }
    public decimal Amount { get; init; }
    public string Reference { get; init; }
    public string Status { get; init; }
    public ulong Version { get; init; }
}
