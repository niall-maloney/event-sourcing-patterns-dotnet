using MediatR;

namespace NiallMaloney.FatEvents.Service.Notifications.Commands;

public record AcknowledgeUserAcceptedNotificationSent(
    string NotificationId,
    string UserId,
    string EmailAddress,
    string Forename,
    string Surname)
    : IRequest;