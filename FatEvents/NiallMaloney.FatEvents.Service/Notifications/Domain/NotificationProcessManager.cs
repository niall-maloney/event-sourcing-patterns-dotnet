using MediatR;
using NiallMaloney.EventSourcing;
using NiallMaloney.EventSourcing.Subscriptions;
using NiallMaloney.FatEvents.Service.Notifications.Commands;
using NiallMaloney.FatEvents.Service.Notifications.Events;
using NiallMaloney.FatEvents.Service.Users.Events;

namespace NiallMaloney.FatEvents.Service.Notifications.Domain;

[SubscriberName("NotificationProcessManager")]
[Subscription("$ce-fat_events.user", begin: CursorFromStream.End)]
[Subscription("$ce-fat_events.notification", begin: CursorFromStream.End)]
public class NotificationProcessManager : SubscriberBase
{
    private readonly IMediator _mediator;

    public NotificationProcessManager(IMediator mediator)
    {
        _mediator = mediator;

        When<UserAccepted>(Handle);
        When<UserAcceptedNotificationSending>(Handle);
    }

    private Task Handle(UserAccepted evnt, EventMetadata metadata)
    {
        return _mediator.Send(
            new SendUserAcceptedNotification(Ids.NewNotificationId(evnt.UserId, NotificationTypes.UserAccepted),
                evnt.UserId, evnt.EmailAddress, evnt.Forename, evnt.Surname));
    }

    private Task Handle(UserAcceptedNotificationSending evnt, EventMetadata metadata)
    {
        return _mediator.Send(
            new AcknowledgeUserAcceptedNotificationSent(evnt.NotificationId, evnt.UserId, evnt.EmailAddress,
                evnt.Forename,
                evnt.Surname));
    }
}
