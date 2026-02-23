using System.Numerics;
using System.Text.Json;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.DeadLetters;
using NodaTime.Extensions;

namespace NiallMaloney.Shared.Cassandra;

public class CassandraDeadLetterRepository : IDeadLetterRepository
{
    private readonly EventSerializer _eventSerializer;
    private readonly Mapper _mapper;

    //CREATE KEYSPACE IF NOT EXISTS "process_manager" WITH replication = {'class':'SimpleStrategy', 'replication_factor' : 1};
    private readonly ISession _session;

    public CassandraDeadLetterRepository(string keyspace, EventSerializer eventSerializer)
    {
        _eventSerializer = eventSerializer;

        var cluster = Cluster.Builder().AddContactPoint("localhost").WithPort(9042).Build();
        _session = cluster.Connect(keyspace);
        _mapper = new Mapper(_session);
        CreateTables();

        MappingConfiguration.Global.Define(
            new Map<DeadLetterRow>()
                .TableName("dead_letters")
                .PartitionKey(d => d.DeadLetterId)
                .Column(d => d.DeadLetterId)
                .Column(d => d.SubscriberName)
                .Column(d => d.SubscriptionStreamId)
                .Column(d => d.SubscriptionEventNumber, map => map.WithDbType<BigInteger>())
                .Column(d => d.EventType)
                .Column(d => d.EventJson, map => map.WithName("event"))
                .Column(d => d.MetadataJson, map => map.WithName("metadata"))
                .Column(d => d.DeadLetteredAt, map => map.WithDbType<DateTimeOffset>())
        );
    }

    public async Task InsertDeadLetter(EventDeadLetter deadLetter)
    {
        var (deadLetterId,
            subscriberName,
            subscriptionStreamId,
            subscriptionEventNumber,
            evnt,
            metadata,
            deadLetteredAt) = deadLetter;
        var evntType = IEvent.GetEventType(evnt.GetType());
        var evntBytes = _eventSerializer.Serialize((IEvent)evnt);
        var metadataBytes = JsonSerializer.SerializeToUtf8Bytes(metadata);
        var query =
            "INSERT INTO dead_letters (deadLetterId, subscriberName, subscriptionStreamId, subscriptionEventNumber, eventType, event, metadata, deadLetteredAt) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        var prepared = await _session.PrepareAsync(query);
        var statement = prepared.Bind(deadLetterId, subscriberName, subscriptionStreamId,
            (BigInteger)subscriptionEventNumber, evntType, evntBytes, metadataBytes,
            deadLetteredAt.ToDateTimeUtc());
        await _session.ExecuteAsync(statement);
    }

    public async Task<EventDeadLetter?> GetDeadLetter(string deadLetterId)
    {
        var row = await _mapper.SingleOrDefaultAsync<DeadLetterRow>(
            "SELECT * FROM dead_letters WHERE deadLetterId=?", deadLetterId);
        return MapToEventDeadLetter(row);
    }

    public async Task<IReadOnlyList<EventDeadLetter>>
        GetAllDeadLettersForSubscriber(string subscriberName)
    {
        CqlQuery<DeadLetterRow> query = new Table<DeadLetterRow>(_session);
        query = query.Where(r => r.SubscriberName == subscriberName).AllowFiltering();
        var rows = await query.ExecuteAsync();
        return rows.Select(MapToEventDeadLetter).ToList();
    }

    public async Task DeleteDeadLetter(string deadLetterId)
    {
        await _mapper.DeleteAsync<DeadLetterRow>("WHERE deadletterid = ?", deadLetterId);
    }

    private EventDeadLetter MapToEventDeadLetter(DeadLetterRow row)
    {
        var evnt = _eventSerializer.Deserialize(row.EventJson, row.EventType) ??
                   throw new InvalidOperationException(
                       $"Could not deserialize event json: {row.EventJson}");
        var metadata = JsonSerializer.Deserialize<EventMetadata>(row.MetadataJson) ??
                       throw new InvalidOperationException(
                           $"Could not deserialize metadata json: {row.MetadataJson}");
        return new EventDeadLetter(row.DeadLetterId, row.SubscriberName, row.SubscriptionStreamId,
            row.SubscriptionEventNumber, evnt, metadata, row.DeadLetteredAt.ToInstant());
    }

    private void CreateTables()
    {
        //CREATE TABLE IF NOT EXISTS dead_letters ( deadLetterId text PRIMARY KEY, subscriberName text, subscriptionStreamId text, subscriptionEventNumber varint, eventType text, eventJson blob, metadataJson blob, deadLetteredAt timestamp)
        _session.Execute(
            "CREATE TABLE IF NOT EXISTS dead_letters ( deadLetterId text PRIMARY KEY, subscriberName text, subscriptionStreamId text, subscriptionEventNumber varint, eventType text, event blob, metadata blob, deadLetteredAt timestamp)"
        );
    }
}
