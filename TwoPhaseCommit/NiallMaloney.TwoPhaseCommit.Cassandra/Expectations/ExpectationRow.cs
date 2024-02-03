namespace NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;

public record ExpectationRow
{
    public string ExpectationId { get; init; } = string.Empty;
    public string Iban { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public ulong Version { get; init; }
}
