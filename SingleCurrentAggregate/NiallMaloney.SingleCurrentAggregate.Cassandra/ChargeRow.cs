namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record ChargeRow
{
    public string? ChargeId { get; set; }
    public string? BillingPeriodId { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public ulong Version { get; set; }
}