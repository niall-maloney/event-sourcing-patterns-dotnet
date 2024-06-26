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
    public static Payment Map(PaymentRow r)
    {
        return new Payment(r.PaymentId, r.Iban, r.Amount, r.Reference, r.Status, r.Version);
    }

    public static IEnumerable<Payment> Map(IEnumerable<PaymentRow> rs)
    {
        return rs.Select(
            r => new Payment(r.PaymentId, r.Iban, r.Amount, r.Reference, r.Status, r.Version)
        );
    }
}