namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record BillingPeriodRow(string BillingPeriodId, string Status, decimal TotalAmount, ulong Version);
