using MediatR;
using NiallMaloney.FatEvents.Cassandra.Notifications;

namespace NiallMaloney.FatEvents.Service.Notifications.Queries;

public record GetNotification(string NotificationId) : IRequest<NotificationRow?>;