using System.Numerics;
using Cassandra.Mapping;
using NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

namespace NiallMaloney.TwoPhaseCommit.Cassandra;

public class CassandraMatchingMappings: Mappings
{
    public CassandraMatchingMappings()
    {
        For<MatchingManagerRow>()
            .TableName("matching_managers")
            .PartitionKey(u => u.MatchingId)
            .Column(u => u.PaymentId)
            .Column(u => u.ExpectationId)
            .Column(u => u.Iban)
            .Column(u => u.Status)
            .Column(u => u.Amount)
            .Column(u => u.Reference)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}