namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record BillingPeriodRow
{
    public string? BillingPeriodId { get; set; }
    public string? CustomerId { get; set; }
    public string? Status { get; set; }
    public decimal TotalAmount { get; set; }
    public ulong Version { get; set; }
}
