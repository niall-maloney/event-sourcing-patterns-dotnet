using NiallMaloney.TwoPhaseCommit.Cassandra;
using NiallMaloney.TwoPhaseCommit.Cassandra.Payments;

namespace NiallMaloney.TwoPhaseCommit.Service.Payments.Controllers.Models;

public record Payment(
    string Id,
    string Iban,
    decimal Amount,
    string Reference,
    string Status,
    ulong Version
)
{
    public static Payment Map(PaymentRow r) =>
        new(r.PaymentId, r.Iban, r.Amount, r.Reference, r.Status, r.Version);

    public static IEnumerable<Payment> Map(IEnumerable<PaymentRow> rs) =>
        rs.Select(
            r => new Payment(r.PaymentId, r.Iban, r.Amount, r.Reference, r.Status, r.Version)
        );
}
