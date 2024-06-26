using NiallMaloney.Shared;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods;

public static class Ids
{
    public static string NewBillingPeriodId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }

    public static string NewChargeId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }

    public static string NewCustomerId(params string[] idempotencyComponents)
    {
        return DeterministicIdFactory.NewId(idempotencyComponents);
    }
}