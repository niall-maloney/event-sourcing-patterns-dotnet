using NiallMaloney.Shared;

namespace NiallMaloney.FatEvents.Service;

public static class Ids
{
    public static string NewUserId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }

    public static string NewNotificationId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }
}