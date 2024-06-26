using NiallMaloney.FatEvents.Cassandra.Notifications;

namespace NiallMaloney.FatEvents.Service.Notifications.Controllers.Models;

public record Notification(string? Id, string? Type, string? Status, ulong? Version)
{
    public static Notification Map(NotificationRow r)
    {
        return new Notification(r.NotificationId, r.Type, r.Status, r.Version);
    }

    public static IEnumerable<Notification> Map(IEnumerable<NotificationRow> rs)
    {
        return rs.Select(
            r => new Notification(r.NotificationId, r.Type, r.Status, r.Version)
        ).ToArray();
    }
}