using NiallMaloney.EventSourcing;

namespace NiallMaloney.FatEvents.Service.Notifications.Events;

[Event("fat_events.user_accepted_notification_sent")]
public record UserAcceptedNotificationSent(
    string NotificationId,
    string UserId,
    string EmailAddress,
    string Forename,
    string Surname) : IEvent;