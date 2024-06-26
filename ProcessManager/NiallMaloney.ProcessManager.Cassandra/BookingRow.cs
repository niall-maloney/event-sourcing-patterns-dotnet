namespace NiallMaloney.ProcessManager.Cassandra;

public record BookingRow
{
    public string? BookingId { get; init; }
    public string? Ledger { get; init; }
    public decimal Amount { get; init; }
    public string? Status { get; init; }
    public ulong Version { get; init; }
}