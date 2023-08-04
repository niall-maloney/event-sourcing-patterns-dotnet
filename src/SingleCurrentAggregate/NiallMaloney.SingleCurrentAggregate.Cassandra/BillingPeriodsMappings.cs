using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.SingleCurrentAggregate.Cassandra;

public class BillingPeriodsMappings : Mappings
{
    public BillingPeriodsMappings()
    {
        For<BillingPeriodRow>()
            .TableName("billing_periods")
            .PartitionKey(u => u.BillingPeriodId)
            .Column(u => u.Status)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}