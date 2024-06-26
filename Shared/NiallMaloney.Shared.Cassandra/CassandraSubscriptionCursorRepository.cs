using System.Numerics;
using Cassandra;
using Cassandra.Mapping;
using NiallMaloney.EventSourcing.Subscriptions;

namespace NiallMaloney.Shared.Cassandra;

public class CassandraSubscriptionCursorRepository : ISubscriptionCursorRepository
{
    private readonly Mapper _mapper;

    //CREATE KEYSPACE IF NOT EXISTS "process_manager" WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};
    private readonly ISession _session;

    public CassandraSubscriptionCursorRepository(string keyspace)
    {
        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(keyspace);
        _mapper = new Mapper(_session);
        CreateTables();
    }

    public async Task<ulong?> GetSubscriptionCursor(string subscriberName, string streamName)
    {
        var row = await _mapper.SingleOrDefaultAsync<SubscriptionCursorRow>(
            "SELECT * FROM subscription_cursors WHERE subscription=?",
            $"{subscriberName}.{streamName}"
        );
        return row?.Position;
    }

    public async Task UpsertSubscriptionCursor(
        string subscriberName,
        string streamName,
        ulong position
    )
    {
        var query = "INSERT INTO subscription_cursors (subscription, position) VALUES (?, ?)";
        var prepared = await _session.PrepareAsync(query);
        var statement = prepared.Bind($"{subscriberName}.{streamName}", (BigInteger)position);
        await _session.ExecuteAsync(statement);
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS process_manager.subscription_cursors ( subscription text PRIMARY KEY, position varint);
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS subscription_cursors ( subscription text PRIMARY KEY, position varint)"
        );
    }
}