namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record BillingPeriodRow
{
    public string? BillingPeriodId { get; init; }
    public string? CustomerId { get; init; }
    public string? Status { get; init; }
    public decimal TotalAmount { get; init; }
    public ulong Version { get; init; }
}