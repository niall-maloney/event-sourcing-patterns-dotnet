namespace NiallMaloney.ProcessManager.Cassandra;

public record BookingRow
{
    public string BookingId { get; set; }
    public string Ledger { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public ulong Version { get; set; }
}