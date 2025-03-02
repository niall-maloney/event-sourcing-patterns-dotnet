using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.PendingCreation.Cassandra.Users;

public class CassandraUsersMappings : Mappings
{
    public CassandraUsersMappings()
    {
        For<UserRow>()
            .TableName("users")
            .PartitionKey(u => u.UserId)
            .Column(u => u.EmailAddress)
            .Column(u => u.Forename)
            .Column(u => u.Surname)
            .Column(u => u.Status)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());

        For<UserDataRow>()
            .TableName("duplicate_user_tracking")
            .PartitionKey(u => u.UserId)
            .Column(u => u.EmailAddress);
    }
}
