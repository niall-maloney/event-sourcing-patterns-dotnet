using MediatR;
using NiallMaloney.FatEvents.Cassandra.Notifications;

namespace NiallMaloney.FatEvents.Service.Notifications.Queries;

public class NotificationQueryHandlers : IRequestHandler<GetNotification, NotificationRow?>,
    IRequestHandler<SearchNotifications, IEnumerable<NotificationRow>>
{
    private readonly INotificationsRepository _repository;

    public NotificationQueryHandlers(INotificationsRepository repository)
    {
        _repository = repository;
    }

    public Task<NotificationRow?> Handle(GetNotification request, CancellationToken cancellationToken)
    {
        return _repository.GetNotification(request.NotificationId);
    }

    public Task<IEnumerable<NotificationRow>>
        Handle(SearchNotifications request, CancellationToken cancellationToken)
    {
        return _repository.SearchNotifications(request.type, request.status);
    }
}