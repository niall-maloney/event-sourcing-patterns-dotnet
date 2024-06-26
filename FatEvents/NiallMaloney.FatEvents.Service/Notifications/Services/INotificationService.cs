namespace NiallMaloney.FatEvents.Service.Notifications.Services;

public interface INotificationService
{
    public Task SendNotification(object payload, string idempotencyKey);
}