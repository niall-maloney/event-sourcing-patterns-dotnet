using MediatR;

namespace NiallMaloney.FatEvents.Service.Notifications.Commands;

public record SendUserAcceptedNotification(
    string NotificationId,
    string UserId,
    string EmailAddress,
    string Forename,
    string Surname)
    : IRequest;