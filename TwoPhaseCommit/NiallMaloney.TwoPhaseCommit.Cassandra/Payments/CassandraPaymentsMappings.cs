using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

public class CassandraPaymentsMappings : Mappings
{
    public CassandraPaymentsMappings()
    {
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