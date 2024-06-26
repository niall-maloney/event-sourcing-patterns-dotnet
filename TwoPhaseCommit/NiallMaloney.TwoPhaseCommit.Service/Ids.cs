using NiallMaloney.Shared;

namespace NiallMaloney.TwoPhaseCommit.Service;

public static class Ids
{
    public static string NewExpectationId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }

    public static string NewMatchingId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }

    public static string NewPaymentId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }
}