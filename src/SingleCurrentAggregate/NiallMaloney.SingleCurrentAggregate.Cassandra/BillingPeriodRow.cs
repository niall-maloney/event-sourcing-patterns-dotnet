using System.Numerics;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public record BillingPeriodRow(string BillingPeriodId, string Status, ulong Version);
