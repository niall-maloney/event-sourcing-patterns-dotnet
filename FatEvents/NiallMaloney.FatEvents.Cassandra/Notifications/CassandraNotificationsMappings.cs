using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.FatEvents.Cassandra.Notifications;

public class CassandraNotificationsMappings : Mappings
{
    public CassandraNotificationsMappings()
    {
        For<NotificationRow>()
            .TableName("notifications")
            .PartitionKey(n => n.NotificationId)
            .Column(n => n.Type)
            .Column(n => n.Status)
            .Column(n => n.Version, map => map.WithDbType<BigInteger>());
    }
}