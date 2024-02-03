using System.Numerics;
using Cassandra.Mapping;
using NiallMaloney.TwoPhaseCommit.Cassandra.Expectations;

namespace NiallMaloney.TwoPhaseCommit.Cassandra;

public class CassandraExpectationsMapping : Mappings
{
    public CassandraExpectationsMapping()
    {
        
        For<ExpectationRow>()
            .TableName("expectations")
            .PartitionKey(u => u.ExpectationId)
            .Column(u => u.Iban)
            .Column(u => u.Status)
            .Column(u => u.Amount)
            .Column(u => u.Reference)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}