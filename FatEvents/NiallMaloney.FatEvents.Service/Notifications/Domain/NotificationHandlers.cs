using MediatR;
using NiallMaloney.EventSourcing.Aggregates;
using NiallMaloney.FatEvents.Service.Notifications.Commands;
using NiallMaloney.FatEvents.Service.Notifications.Services;

namespace NiallMaloney.FatEvents.Service.Notifications.Domain;

public class NotificationHandlers : IRequestHandler<SendUserAcceptedNotification>,
    IRequestHandler<AcknowledgeUserAcceptedNotificationSent>
{
    private readonly INotificationService _notificationService;
    private readonly AggregateRepository _repository;

    public NotificationHandlers(AggregateRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task Handle(AcknowledgeUserAcceptedNotificationSent request, CancellationToken cancellationToken)
    {
        var (notificationId, userId, emailAddress, forename, surname) = request;
        var notification = await _repository.LoadAggregate<Notification>(notificationId);
        notification.AcknowledgeUserAcceptedNotificationSent(userId, emailAddress, forename, surname);
        await _repository.SaveAggregate(notification);
    }

    public async Task Handle(SendUserAcceptedNotification request, CancellationToken cancellationToken)
    {
        var (notificationId, userId, emailAddress, forename, surname) = request;
        var notification = await _repository.LoadAggregate<Notification>(notificationId);
        await _notificationService.SendNotification(
            new { Message = $"Welcome {forename} {surname}!", EmailAddress = emailAddress },
            notificationId);
        notification.SendUserAcceptedNotification(userId, emailAddress, forename, surname);
        await _repository.SaveAggregate(notification);
    }
}