namespace NiallMaloney.FatEvents.Cassandra.Notifications;

public interface INotificationsRepository
{
    public Task AddNotification(NotificationRow notification);
    public Task UpdateNotification(NotificationRow notification);
    Task<NotificationRow?> GetNotification(string notificationId);
    Task<IEnumerable<NotificationRow>> SearchNotifications(string? type, string? status);
}