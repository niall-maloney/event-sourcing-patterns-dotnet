using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record BillingPeriod(string Id, string Status, ulong Version)
{
    public static BillingPeriod Map(BillingPeriodRow r) => new(r.BillingPeriodId, r.Status, r.Version);

    public static IEnumerable<BillingPeriod> Map(IEnumerable<BillingPeriodRow> rs) =>
        rs.Select(r => new BillingPeriod(r.BillingPeriodId, r.Status, r.Version));
}
