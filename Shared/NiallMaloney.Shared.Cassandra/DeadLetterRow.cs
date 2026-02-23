namespace NiallMaloney.Shared.Cassandra;

public record DeadLetterRow
{
    public required string DeadLetterId { get; set; }
    public required string SubscriberName { get; set; }
    public required string SubscriptionStreamId { get; set; }
    public required ulong SubscriptionEventNumber { get; set; }
    public required string EventType { get; set; }
    public required byte[] EventJson { get; set; }
    public required byte[] MetadataJson { get; set; }
    public required DateTimeOffset DeadLetteredAt { get; set; }
};
