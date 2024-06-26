using NiallMaloney.ProcessManager.Cassandra;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Controllers.Models;

public record Booking(string? Id, string? Ledger, string? Status, decimal Amount, ulong Version)
{
    public static Booking Map(BookingRow r)
    {
        return new Booking(r.BookingId, r.Ledger, r.Status, r.Amount, r.Version);
    }

    public static IEnumerable<Booking> Map(IEnumerable<BookingRow> rs)
    {
        return rs.Select(r => new Booking(r.BookingId, r.Ledger, r.Status, r.Amount, r.Version));
    }
}
