using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.FatEvents.Service.Notifications.Events;

namespace NiallMaloney.FatEvents.Service.Notifications.Domain;

[Category("fat_events.notification")]
public class Notification : Aggregate
{
    private bool _hasBeenSent;
    private bool _hasStartedSending;
    private string _type = string.Empty;

    public Notification()
    {
        When<UserAcceptedNotificationSending>(Apply);
        When<UserAcceptedNotificationSent>(Apply);
    }

    public void SendUserAcceptedNotification(string userId, string emailAddress, string forename, string surname)
    {
        AssertExpectedTypeOrThrow(NotificationTypes.UserAccepted);

        if (_hasStartedSending || _hasBeenSent)
        {
            return;
        }

        RaiseEvent(new UserAcceptedNotificationSending(Id, userId, emailAddress, forename, surname));
    }

    public void AcknowledgeUserAcceptedNotificationSent(string userId, string emailAddress, string forename,
        string surname)
    {
        AssertExpectedTypeOrThrow(NotificationTypes.UserAccepted);

        if (_hasBeenSent)
        {
            return;
        }

        RaiseEvent(new UserAcceptedNotificationSent(Id, userId, emailAddress, forename, surname));
    }

    private void AssertExpectedTypeOrThrow(string type)
    {
        if (string.IsNullOrEmpty(_type) || _type == type)
        {
            return;
        }

        //todo error message / specific exception
        throw new InvalidOperationException();
    }

    private void Apply(UserAcceptedNotificationSending evnt)
    {
        _hasStartedSending = true;
        _type = NotificationTypes.UserAccepted;
    }

    private void Apply(UserAcceptedNotificationSent evnt)
    {
        _hasBeenSent = true;
    }
}