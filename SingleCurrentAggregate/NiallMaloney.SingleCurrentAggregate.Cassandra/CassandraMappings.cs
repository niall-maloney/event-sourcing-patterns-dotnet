using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public class CassandraMappings : Mappings
{
    public CassandraMappings()
    {
        For<BillingPeriodRow>()
            .TableName("billing_periods")
            .PartitionKey(u => u.BillingPeriodId)
            .Column(u => u.CustomerId)
            .Column(u => u.Status)
            .Column(u => u.TotalAmount)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());

        For<ChargeRow>()
            .TableName("charges")
            .PartitionKey(u => u.ChargeId)
            .Column(u => u.Amount)
            .Column(u => u.BillingPeriodId)
            .Column(u => u.Status)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}