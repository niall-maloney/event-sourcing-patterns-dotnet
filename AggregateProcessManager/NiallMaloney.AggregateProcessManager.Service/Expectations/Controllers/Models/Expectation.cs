using NiallMaloney.AggregateProcessManager.Cassandra;

namespace NiallMaloney.AggregateProcessManager.Service.Expectations.Controllers.Models;

public record Expectation(string Id, string Iban, decimal Amount, string Reference, string Status, ulong Version)
{
    public static Expectation Map(ExpectationRow r) =>
        new(r.ExpectationId, r.Iban, r.Amount, r.Reference, r.Status, r.Version);

    public static IEnumerable<Expectation> Map(IEnumerable<ExpectationRow> rs) =>
        rs.Select(r => new Expectation(r.ExpectationId, r.Iban, r.Amount, r.Reference, r.Status, r.Version));
}