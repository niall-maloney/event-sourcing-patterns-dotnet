namespace NiallMaloney.ProcessManager.Cassandra;

public interface ILedgersRepository
{
    public Task<IEnumerable<LedgerRow>> GetLedgers();
    public Task<LedgerRow?> GetLedger(string ledger);
    public Task<decimal?> GetBalance(string ledger);
    public Task<(bool, decimal)> UpdateBalance(string ledger, decimal updatedBalance, decimal? currentBalance);
}
