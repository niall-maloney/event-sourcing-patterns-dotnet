using NiallMaloney.Shared;

namespace NiallMaloney.PendingCreation.Service;

public static class Ids
{
    public static string NewUserId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }
}