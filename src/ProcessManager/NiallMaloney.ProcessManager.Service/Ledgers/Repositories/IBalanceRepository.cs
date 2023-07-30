using NiallMaloney.ProcessManager.Service.Ledgers.Models;

namespace NiallMaloney.ProcessManager.Service.Ledgers.Repositories;

public interface IBalanceRepository
{
    public Task<Balance?> GetBalance(string ledger);
    public Task<(bool, decimal)> UpdateBalance(string ledger, decimal amount);
}
