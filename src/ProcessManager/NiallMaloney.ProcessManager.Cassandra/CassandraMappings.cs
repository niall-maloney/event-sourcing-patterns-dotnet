using System.Numerics;
using Cassandra.Mapping;

namespace NiallMaloney.ProcessManager.Cassandra;

public class CassandraMappings : Mappings
{
    public CassandraMappings()
    {
        For<BookingRow>()
            .TableName("bookings")
            .PartitionKey(u => u.BookingId)
            .Column(u => u.Ledger)
            .Column(u => u.Amount)
            .Column(u => u.Status)
            .Column(u => u.Version, map => map.WithDbType<BigInteger>());
    }
}