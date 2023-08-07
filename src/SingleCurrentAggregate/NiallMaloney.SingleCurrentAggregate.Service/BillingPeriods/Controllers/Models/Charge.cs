using NiallMaloney.SingleCurrentAggregate.Cassandra;

namespace NiallMaloney.SingleCurrentAggregate.Service.BillingPeriods.Controllers.Models;

public record Charge(string? Id, string? BillingPeriodId, string? Status, decimal Amount, ulong Version)
{
    public static Charge Map(ChargeRow r) => new(r.ChargeId, r.BillingPeriodId, r.Status, r.Amount, r.Version);

    public static IEnumerable<Charge> Map(IEnumerable<ChargeRow> rs) =>
        rs.Select(r => new Charge(r.ChargeId, r.BillingPeriodId, r.Status, r.Amount, r.Version));
}
