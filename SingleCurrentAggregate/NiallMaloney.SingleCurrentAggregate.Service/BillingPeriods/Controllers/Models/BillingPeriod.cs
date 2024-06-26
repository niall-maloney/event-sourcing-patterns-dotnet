using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record BillingPeriod(
    string? Id,
    string? CustomerId,
    string? Status,
    decimal TotalAmount,
    ulong Version
)
{
    public static BillingPeriod Map(BillingPeriodRow r)
    {
        return new BillingPeriod(r.BillingPeriodId, r.CustomerId, r.Status, r.TotalAmount, r.Version);
    }

    public static IEnumerable<BillingPeriod> Map(IEnumerable<BillingPeriodRow> rs)
    {
        return rs.Select(
            r =>
                new BillingPeriod(
                    r.BillingPeriodId,
                    r.CustomerId,
                    r.Status,
                    r.TotalAmount,
                    r.Version
                )
        );
    }
}