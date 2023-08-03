namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods;

public static class Ids
{
    public static string NewBillingPeriodId() => NewGuidString();
    public static string NewChargeId() => NewGuidString();
    private static string NewGuidString() => Guid.NewGuid().ToString();
}