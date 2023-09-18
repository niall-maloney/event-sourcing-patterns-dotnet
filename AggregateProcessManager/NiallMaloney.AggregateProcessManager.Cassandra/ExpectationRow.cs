namespace NiallMaloney.AggregateProcessManager.Cassandra;

public record ExpectationRow
{
    public string ExpectationId { get; init; }
    public string Iban { get; init; }
    public decimal Amount { get; init; }
    public string Reference { get; init; }
    public string Status { get; init; }
    public ulong Version { get; init; }
}
