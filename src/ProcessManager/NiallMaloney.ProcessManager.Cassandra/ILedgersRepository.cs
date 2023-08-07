namespace NiallMaloney.ProcessManager.Cassandra;

public interface ILedgersRepository
{
    public Task<IEnumerable<LedgerRow>> GetLedgers();
    public Task<LedgerRow?> GetLedger(string ledger);

    public Task UpdateBalance(
        string ledger,
        decimal newPendingBalance,
        decimal newCommittedBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition);

    public Task UpdateCommittedBalance(
        string ledger,
        decimal newBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition);

    public Task UpdatePendingBalance(
        string ledger,
        decimal newBalance,
        ulong newStreamPosition,
        ulong? lastStreamPosition);
}
