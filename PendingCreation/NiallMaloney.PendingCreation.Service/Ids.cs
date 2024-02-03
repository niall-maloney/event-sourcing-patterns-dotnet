using NiallMaloney.Shared;

namespace NiallMaloney.PendingCreation.Service;

public static class Ids
{
    public static string NewUserId(params string[] idempotencyComponents) =>
        DeterministicIdFactory.NewId(idempotencyComponents);
}
