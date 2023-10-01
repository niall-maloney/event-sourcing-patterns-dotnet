using NiallMaloney.Shared;

namespace NiallMaloney.TwoPhaseCommit.Service;

public static class Ids
{
    public static string NewExpectationId(params string[] idempotencyComponents) =>
        DeterministicIdFactory.NewId(idempotencyComponents);

    public static string NewMatchingId(params string[] idempotencyComponents) =>
        DeterministicIdFactory.NewId(idempotencyComponents);

    public static string NewPaymentId(params string[] idempotencyComponents) =>
        DeterministicIdFactory.NewId(idempotencyComponents);
}
