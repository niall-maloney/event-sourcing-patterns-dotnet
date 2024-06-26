namespace NiallMaloney.FatEvents.Cassandra.Notifications;

public record NotificationRow
{
    public string? NotificationId { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public ulong Version { get; init; }
}