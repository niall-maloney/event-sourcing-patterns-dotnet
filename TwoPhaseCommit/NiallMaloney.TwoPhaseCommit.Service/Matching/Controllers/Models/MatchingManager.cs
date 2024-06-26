using NiallMaloney.TwoPhaseCommit.Cassandra.Matching;

namespace NiallMaloney.TwoPhaseCommit.Service.Matching.Controllers.Models;

public record MatchingManager(
    string Id,
    string PaymentId,
    string ExpectationId,
    string Iban,
    decimal Amount,
    string Reference,
    string Status,
    ulong Version
)
{
    public static MatchingManager Map(MatchingManagerRow r)
    {
        return new MatchingManager(r.MatchingId, r.PaymentId, r.ExpectationId, r.Iban, r.Amount, r.Reference, r.Status,
            r.Version);
    }

    public static IEnumerable<MatchingManager> Map(IEnumerable<MatchingManagerRow> rs)
    {
        return rs.Select(
            r => new MatchingManager(r.MatchingId, r.PaymentId, r.ExpectationId, r.Iban, r.Amount, r.Reference,
                r.Status, r.Version)
        );
    }
}