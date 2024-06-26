namespace NiallMaloney.FatEvents.Service.Notifications.Services;

public class NullNotificationService : INotificationService
{
    public Task SendNotification(object payload, string idempotencyKey)
    {
        return Task.CompletedTask;
    }
}