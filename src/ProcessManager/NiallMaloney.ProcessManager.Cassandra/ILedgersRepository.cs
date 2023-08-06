namespace NiallMaloney.ProcessManager.Cassandra;

public interface ILedgersRepository
{
    public Task<IEnumerable<LedgerRow>> GetLedgers();
    public Task<LedgerRow?> GetLedger(string ledger);
    public Task<(decimal?, decimal?)> GetBalance(string ledger);

    public Task UpdateBalance(
        string ledger,
        decimal updatedPendingBalance,
        decimal? currentPendingBalance,
        decimal updatedCommittedBalance,
        decimal? currentCommittedBalance);
}
