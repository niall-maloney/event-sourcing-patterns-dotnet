using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Projections;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.FatEvents.Cassandra.Notifications;
using NiallMaloney.FatEvents.Service.Notifications.Domain;
using NiallMaloney.FatEvents.Service.Notifications.Events;

namespace NiallMaloney.FatEvents.Service.Notifications.Projections;

[SubscriberName("NotificationsProjection")]
[Subscription("$ce-fat_events.notification")]
public class NotificationsProjection : Projection
{
    private readonly INotificationsRepository _repository;

    public NotificationsProjection(INotificationsRepository repository)
    {
        _repository = repository;

        When<UserAcceptedNotificationSending>(Handle);
        When<UserAcceptedNotificationSent>(Handle);
    }

    private async Task Handle(UserAcceptedNotificationSending evnt, EventMetadata metadata)
    {
        await AddNotification(evnt.NotificationId, NotificationTypes.UserAccepted, metadata.StreamPosition);
    }

    private async Task Handle(UserAcceptedNotificationSent evnt, EventMetadata metadata)
    {
        await UpdateNotification(evnt.NotificationId, "Sent", metadata.StreamPosition);
    }

    private async Task AddNotification(string notificationId, string type, ulong streamPosition)
    {
        var notification = await _repository.GetNotification(notificationId);
        if (notification is not null)
        {
            return;
        }

        await _repository.AddNotification(new NotificationRow
        {
            NotificationId = notificationId,
            Type = type,
            Status = "Sending",
            Version = streamPosition,
        });
    }

    private async Task UpdateNotification(string notificationId, string newStatus, ulong newStreamPosition)
    {
        var notification = await _repository.GetNotification(notificationId);
        if (notification is null || !TryUpdateVersion(notification, newStreamPosition, out notification))
        {
            return;
        }

        notification = notification with { Status = newStatus };
        await _repository.UpdateNotification(notification);
    }

    private bool TryUpdateVersion(NotificationRow notification, ulong newVersion, out NotificationRow newNotification)
    {
        var expectedVersion = newVersion - 1;
        var actualVersion = notification.Version;
        if (actualVersion >= newVersion)
        {
            newNotification = notification;
            return false;
        }

        if (actualVersion != expectedVersion)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected {expectedVersion} actual {actualVersion}"
            );
        }

        newNotification = notification with { Version = newVersion };
        return true;
    }
}