using MediatR;
using NiallMaloney.FatEvents.Cassandra.Notifications;

namespace NiallMaloney.FatEvents.Service.Notifications.Queries;

public record SearchNotifications(string type, string status) : IRequest<IEnumerable<NotificationRow>>;