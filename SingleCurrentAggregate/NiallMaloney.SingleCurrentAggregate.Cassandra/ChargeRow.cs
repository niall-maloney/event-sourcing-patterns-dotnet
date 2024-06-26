namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record ChargeRow
{
    public string? ChargeId { get; init; }
    public string? BillingPeriodId { get; init; }
    public decimal Amount { get; init; }
    public string? Status { get; init; }
    public ulong Version { get; init; }
}