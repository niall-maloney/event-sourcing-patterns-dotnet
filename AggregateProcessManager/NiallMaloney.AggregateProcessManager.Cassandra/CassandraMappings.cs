using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.AggregateProcessManager.Cassandra;

public class CassandraMappings : Mappings
{
    public CassandraMappings()
    {
        For<ExpectationRow>()
            .TableName("expectations")
            .PartitionKey(u => u.ExpectationId)
            .Column(u => u.Iban)
            .Column(u => u.Status)
            .Column(u => u.Amount)
            .Column(u => u.Reference)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());

        For<PaymentRow>()
            .TableName("payments")
            .PartitionKey(u => u.PaymentId)
            .Column(u => u.Iban)
            .Column(u => u.Status)
            .Column(u => u.Amount)
            .Column(u => u.Reference)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}
