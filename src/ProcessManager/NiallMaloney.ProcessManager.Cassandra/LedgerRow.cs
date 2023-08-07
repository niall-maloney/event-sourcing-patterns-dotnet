namespace NiallMaloney.ProcessManager.Cassandra;

public record LedgerRow
{
    public string Ledger { get; set; }
    public decimal PendingAmount { get; set; }
    public decimal CommittedAmount { get; set; }
    public ulong LastStreamPosition { get; set; }
}
