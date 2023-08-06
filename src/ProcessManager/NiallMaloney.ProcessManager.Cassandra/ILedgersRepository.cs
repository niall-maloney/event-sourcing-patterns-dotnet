namespace NiallMaloney.ProcessManager.Cassandra;

public interface ILedgersRepository
{
    public Task<IEnumerable<LedgerRow>> GetLedgers();
    public Task<LedgerRow?> GetLedger(string ledger);
    public Task<(decimal? PendingBalance, decimal? CommittedBalance)> GetBalance(string ledger);

    public Task UpdateBalance(
        string ledger,
        decimal updatedPendingBalance,
        decimal? currentPendingBalance,
        decimal updatedCommittedBalance,
        decimal? currentCommittedBalance);

    public Task UpdateCommittedBalance(string ledger, decimal updatedBalance, decimal? currentBalance);
    public Task UpdatePendingBalance(string ledger, decimal updatedBalance, decimal? currentBalance);
}
